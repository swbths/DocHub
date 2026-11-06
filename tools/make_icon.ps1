$ErrorActionPreference = 'Stop'
Add-Type -AssemblyName System.Drawing

function New-RoundedPath([single]$x, [single]$y, [single]$w, [single]$h, [single]$r) {
    $d = $r * 2
    if ($d -gt $w) { $d = $w }
    if ($d -gt $h) { $d = $h }
    $p = [System.Drawing.Drawing2D.GraphicsPath]::new()
    $p.AddArc($x, $y, $d, $d, 180, 90)
    $p.AddArc(($x + $w - $d), $y, $d, $d, 270, 90)
    $p.AddArc(($x + $w - $d), ($y + $h - $d), $d, $d, 0, 90)
    $p.AddArc($x, ($y + $h - $d), $d, $d, 90, 90)
    $p.CloseFigure()
    return $p
}

# Flat brand mark: brand-blue rounded square, tilted white sheets, amber merge badge.
# Below 48px the tilt and text lines are removed so taskbar icons stay crisp.
function New-LogoPng([int]$size) {
    $bmp = [System.Drawing.Bitmap]::new($size, $size, [System.Drawing.Imaging.PixelFormat]::Format32bppArgb)
    $g = [System.Drawing.Graphics]::FromImage($bmp)
    $g.SmoothingMode = [System.Drawing.Drawing2D.SmoothingMode]::AntiAlias
    $g.PixelOffsetMode = [System.Drawing.Drawing2D.PixelOffsetMode]::HighQuality
    $g.Clear([System.Drawing.Color]::Transparent)

    $margin = [single][Math]::Max(0.5, ($size * 0.035))
    $x = $margin
    $y = $margin
    $u = [single]($size - 2 * $margin)

    $accent = [System.Drawing.Color]::FromArgb(255, 47, 107, 255)
    $badge = [System.Drawing.Color]::FromArgb(255, 255, 189, 61)
    $badgeGlyph = [System.Drawing.Color]::FromArgb(255, 35, 49, 74)

    $bg = New-RoundedPath $x $y $u $u ([single]($u * 0.235))
    $bgBrush = [System.Drawing.SolidBrush]::new($accent)
    $g.FillPath($bgBrush, $bg)
    $bgBrush.Dispose()
    $bg.Dispose()

    $simple = $size -lt 48

    if ($simple) {
        $backRect = [System.Drawing.RectangleF]::new(($x + $u * 0.235), ($y + $u * 0.300), ($u * 0.44), ($u * 0.50))
        $backPath = New-RoundedPath $backRect.X $backRect.Y $backRect.Width $backRect.Height ([single]($u * 0.058))
        $backBrush = [System.Drawing.SolidBrush]::new([System.Drawing.Color]::FromArgb(150, 255, 255, 255))
        $g.FillPath($backBrush, $backPath)
        $backBrush.Dispose(); $backPath.Dispose()

        $frontRect = [System.Drawing.RectangleF]::new(($x + $u * 0.365), ($y + $u * 0.220), ($u * 0.47), ($u * 0.56))
        $frontPath = New-RoundedPath $frontRect.X $frontRect.Y $frontRect.Width $frontRect.Height ([single]($u * 0.062))
        $frontBrush = [System.Drawing.SolidBrush]::new([System.Drawing.Color]::FromArgb(255, 255, 255, 255))
        $g.FillPath($frontBrush, $frontPath)
        $frontBrush.Dispose(); $frontPath.Dispose()
    }
    else {
        # Back sheet (semi-transparent white, -9 degrees)
        $state = $g.Save()
        $g.TranslateTransform(($x + $u * 0.415), ($y + $u * 0.500))
        $g.RotateTransform(-9)
        $backRect = [System.Drawing.RectangleF]::new((- $u * 0.40 / 2), (- $u * 0.50 / 2), ($u * 0.40), ($u * 0.50))
        $backPath = New-RoundedPath $backRect.X $backRect.Y $backRect.Width $backRect.Height ([single]($u * 0.058))
        $backBrush = [System.Drawing.SolidBrush]::new([System.Drawing.Color]::FromArgb(120, 255, 255, 255))
        $g.FillPath($backBrush, $backPath)
        $backBrush.Dispose(); $backPath.Dispose()
        $g.Restore($state)

        # Front sheet (solid white, +7 degrees) with three accent text lines
        $state = $g.Save()
        $g.TranslateTransform(($x + $u * 0.545), ($y + $u * 0.470))
        $g.RotateTransform(7)
        $frontRect = [System.Drawing.RectangleF]::new((- $u * 0.45 / 2), (- $u * 0.54 / 2), ($u * 0.45), ($u * 0.54))
        $frontPath = New-RoundedPath $frontRect.X $frontRect.Y $frontRect.Width $frontRect.Height ([single]($u * 0.058))
        $frontBrush = [System.Drawing.SolidBrush]::new([System.Drawing.Color]::FromArgb(248, 255, 255, 255))
        $g.FillPath($frontBrush, $frontPath)
        $frontBrush.Dispose(); $frontPath.Dispose()

        $linePen = [System.Drawing.Pen]::new($accent, [single][Math]::Max(1.0, ($u * 0.024)))
        $linePen.StartCap = [System.Drawing.Drawing2D.LineCap]::Round
        $linePen.EndCap = [System.Drawing.Drawing2D.LineCap]::Round
        $half = $u * 0.45 / 2
        $pad = $u * 0.075
        $lineLength = $half * 2 - $pad * 2
        for ($i = 0; $i -lt 3; $i++) {
            $lineY = $u * 0.54 * ($i - 1) * 0.30
            $endX = - $half + $pad + $lineLength
            if ($i -eq 2) { $endX = - $half + $pad + $lineLength * 0.68 }
            $g.DrawLine($linePen, (- $half + $pad), $lineY, $endX, $lineY)
        }
        $linePen.Dispose()
        $g.Restore($state)
    }

    # Amber merge badge: accent-colored knockout ring, amber disc, dark plus
    $badgeX = $x + $u * 0.800
    $badgeY = $y + $u * 0.800
    $ringR = $u * 0.200
    $fillR = $u * 0.155
    $ringBrush = [System.Drawing.SolidBrush]::new($accent)
    $g.FillEllipse($ringBrush, ($badgeX - $ringR), ($badgeY - $ringR), ($ringR * 2), ($ringR * 2))
    $ringBrush.Dispose()
    $badgeBrush = [System.Drawing.SolidBrush]::new($badge)
    $g.FillEllipse($badgeBrush, ($badgeX - $fillR), ($badgeY - $fillR), ($fillR * 2), ($fillR * 2))
    $badgeBrush.Dispose()

    $arm = $u * $(if ($simple) { 0.078 } else { 0.070 })
    $plusPen = [System.Drawing.Pen]::new($badgeGlyph, [single][Math]::Max(1.0, ($u * 0.045)))
    $plusPen.StartCap = [System.Drawing.Drawing2D.LineCap]::Round
    $plusPen.EndCap = [System.Drawing.Drawing2D.LineCap]::Round
    $g.DrawLine($plusPen, ($badgeX - $arm), $badgeY, ($badgeX + $arm), $badgeY)
    $g.DrawLine($plusPen, $badgeX, ($badgeY - $arm), $badgeX, ($badgeY + $arm))
    $plusPen.Dispose()

    $ms = [System.IO.MemoryStream]::new()
    $bmp.Save($ms, [System.Drawing.Imaging.ImageFormat]::Png)
    $bytes = $ms.ToArray()

    $ms.Dispose(); $g.Dispose(); $bmp.Dispose()
    return $bytes
}

$sizes = @(256, 128, 64, 48, 40, 32, 24, 16)
$images = [System.Collections.ArrayList]::new()
foreach ($s in $sizes) {
    $img = [byte[]](New-LogoPng $s)
    if ($img.Length -lt 100) { throw "PNG for size $s too small: $($img.Length)" }
    [void]$images.Add($img)
}

$ms = [System.IO.MemoryStream]::new()
$bw = [System.IO.BinaryWriter]::new($ms)
$bw.Write([UInt16]0)
$bw.Write([UInt16]1)
$bw.Write([UInt16]$sizes.Count)
$offset = 6 + 16 * $sizes.Count
for ($i = 0; $i -lt $sizes.Count; $i++) {
    $s = $sizes[$i]
    $len = $images[$i].Length
    $dim = 0
    if ($s -lt 256) { $dim = $s }
    $bw.Write([Byte]$dim)
    $bw.Write([Byte]$dim)
    $bw.Write([Byte]0)
    $bw.Write([Byte]0)
    $bw.Write([UInt16]1)
    $bw.Write([UInt16]32)
    $bw.Write([UInt32]$len)
    $bw.Write([UInt32]$offset)
    $offset += $len
}
foreach ($img in $images) { $bw.Write($img) }
$bw.Flush()

$root = Split-Path $PSScriptRoot -Parent
$iconPath = Join-Path $root 'app.ico'
[System.IO.File]::WriteAllBytes($iconPath, $ms.ToArray())
$bw.Dispose(); $ms.Dispose()
Write-Host "app.ico created: $iconPath ($((Get-Item $iconPath).Length) bytes)"
