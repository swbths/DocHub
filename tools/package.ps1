# Build script for DocHub: installer .exe + portable package (no compression).
# Usage: powershell -ExecutionPolicy Bypass -File tools\package.ps1
# Keep this file pure ASCII: Windows PowerShell 5 may misread UTF-8 without BOM.
#
# Installer route: the project ships its own .NET self-contained installer
# (installer\Setup.csproj). NSIS is NOT used any more: it is impossible to make
# NSIS ask "abort / retry / ignore" go away when a single file cannot be written,
# whereas the custom installer records failures and reports them once at the end.
$ErrorActionPreference = 'Stop'

$root = Split-Path -Parent $PSScriptRoot
$version = '4.5.24'

# Product names are built from Unicode code points to stay ASCII-safe.
$appName = [string]([char]0x6587) + [char]0x67A2
$greenName = [string]([char]0x7EFF) + [char]0x8272 + [char]0x7248
$setupWord = [string]([char]0x5B89) + [char]0x88C5 + [char]0x7A0B + [char]0x5E8F

# Launcher keeps an ASCII file name (same name as the app, in a separate folder).
$exeName = 'DocHub.exe'
$build = Join-Path $root 'build'
$layoutDir = Join-Path $build 'layout'
$layoutApp = Join-Path $layoutDir 'app'
$launcherDir = Join-Path $build 'launcher'
$setupPublishDir = Join-Path $build 'setup'
$dist = Join-Path $root 'dist'
$payloadZip = Join-Path $root 'installer\payload.zip'
$launcherSource = Join-Path $root 'installer\launcher\Launcher.cs'
$launcherInfo = Join-Path $root 'installer\launcher\LauncherAssemblyInfo.cs'
$iconFile = Join-Path $root 'app.ico'
$csc = Join-Path $env:WINDIR 'Microsoft.NET\Framework64\v4.0.30319\csc.exe'

$running = Get-Process -Name 'DocHub' -ErrorAction SilentlyContinue
if ($running) {
    Write-Warning 'DocHub.exe seems to be running. Close it before packaging to avoid locked files.'
}

function New-ZipDirectory {
    param(
        [string]$ZipPath,
        [string]$SourceDir,
        [bool]$NoCompression
    )

    if (Test-Path $ZipPath) { Remove-Item $ZipPath -Force }
    Add-Type -AssemblyName System.IO.Compression | Out-Null
    Add-Type -AssemblyName System.IO.Compression.FileSystem | Out-Null

    $stream = [System.IO.File]::Create($ZipPath)
    try {
        $archive = New-Object System.IO.Compression.ZipArchive($stream, [System.IO.Compression.ZipArchiveMode]::Create)
        try {
            if ($NoCompression) {
                $level = [System.IO.Compression.CompressionLevel]::NoCompression
            } else {
                $level = [System.IO.Compression.CompressionLevel]::Optimal
            }

            $basePath = (Resolve-Path $SourceDir).Path.TrimEnd('\') + '\'
            Get-ChildItem $SourceDir -Recurse -File | ForEach-Object {
                $relative = $_.FullName.Substring($basePath.Length).Replace('\', '/')
                $entry = $archive.CreateEntry($relative, $level)
                $output = $entry.Open()
                try {
                    $input = [System.IO.File]::OpenRead($_.FullName)
                    try { $input.CopyTo($output) } finally { $input.Dispose() }
                } finally { $output.Dispose() }
            }
        } finally { $archive.Dispose() }
    } finally { $stream.Dispose() }
}

Write-Host 'Cleaning build output...'
if (Test-Path $build) { Remove-Item $build -Recurse -Force }
if (Test-Path $dist) { Remove-Item $dist -Recurse -Force }
if (Test-Path $payloadZip) { Remove-Item $payloadZip -Force }
New-Item -ItemType Directory -Path $layoutApp -Force | Out-Null
New-Item -ItemType Directory -Path $dist -Force | Out-Null

Write-Host 'Publishing application (self-contained, runtime included)...'
dotnet publish (Join-Path $root 'DocHub.csproj') -c Release -o $layoutApp -p:DebugType=None -p:DebugSymbols=false
$appExe = Join-Path $layoutApp 'DocHub.exe'
if (-not (Test-Path $appExe)) { throw 'Publish failed: DocHub.exe not found.' }
Write-Host ('Published files: ' + (Get-ChildItem $layoutApp -Recurse -File).Count)

# MIT 要求版权与许可声明随所有副本一同分发：把两份声明文件放进载荷。
# layout 根目录会原样成为安装根目录 / 绿色版 zip 根目录，layout\app 则是
# DocHub.exe 所在的应用目录，两处各放一份。
# MIT requires the copyright + permission notice to ship with every copy; the
# two notice files therefore travel inside the payload (install root, zip root
# and the app folder next to DocHub.exe). Keep the code ASCII-safe.
Write-Host 'Bundling LICENSE and NOTICE.md...'
foreach ($noticeName in @('LICENSE', 'NOTICE.md')) {
    $noticeSource = Join-Path $root $noticeName
    if (-not (Test-Path $noticeSource)) { throw ('Missing notice file: ' + $noticeSource) }
    Copy-Item $noticeSource (Join-Path $layoutDir $noticeName) -Force
    Copy-Item $noticeSource (Join-Path $layoutApp $noticeName) -Force
}

# Keep support libraries in app\lib so the program folder stays tidy.
Write-Host 'Grouping support libraries into app\lib...'
$libDir = Join-Path $layoutApp 'lib'
New-Item -ItemType Directory -Path $libDir -Force | Out-Null
Get-ChildItem $layoutApp -File | Where-Object { $_.Name -like 'PdfSharp*.dll' } |
    ForEach-Object { Move-Item $_.FullName (Join-Path $libDir $_.Name) -Force }

# Tiny launcher at the install root so the top level only shows one small exe.
Write-Host 'Compiling launcher...'

# Launcher and installer carry their own version metadata; keep both in sync
# with $version so a version bump in this script propagates automatically.
# Both files are UTF-8 (no BOM) with Chinese text: read/write via .NET APIs,
# never via Get-Content/Set-Content, which assume ANSI in Windows PowerShell 5.
$utf8NoBom = New-Object System.Text.UTF8Encoding($false)
$launcherInfoPath = Join-Path $root 'installer\launcher\LauncherAssemblyInfo.cs'
$launcherInfoText = [System.IO.File]::ReadAllText($launcherInfoPath, $utf8NoBom)
$launcherInfoText = [regex]::Replace(
    $launcherInfoText, '"\d+\.\d+\.\d+(?:\.\d+)?"', '"' + $version + '"')
[System.IO.File]::WriteAllText($launcherInfoPath, $launcherInfoText, $utf8NoBom)
$setupProject = Join-Path $root 'installer\Setup.csproj'
$setupText = [System.IO.File]::ReadAllText($setupProject, $utf8NoBom)
$setupText = [regex]::Replace(
    $setupText, '<Version>\d+\.\d+\.\d+</Version>', '<Version>' + $version + '</Version>')
[System.IO.File]::WriteAllText($setupProject, $setupText, $utf8NoBom)

New-Item -ItemType Directory -Path $launcherDir -Force | Out-Null
if (-not (Test-Path $csc)) { throw 'csc.exe not found (.NET Framework 4.x required).' }
$launcherOut = Join-Path $launcherDir $exeName
if (Test-Path $launcherOut) { [System.IO.File]::Delete($launcherOut) }
& $csc /nologo /target:winexe /optimize /codepage:65001 "/win32icon:$iconFile" `
    "/out:$launcherOut" $launcherSource $launcherInfo
if ($LASTEXITCODE -ne 0 -or -not (Test-Path $launcherOut)) { throw 'Launcher compile failed.' }
Copy-Item $launcherOut (Join-Path $layoutDir $exeName) -Force

# Strip the "downloaded from the internet" mark (Zone.Identifier) from every
# layout file before it is archived: the payload must be mark-free so the
# extracted program never trips the Windows attachment manager again.
Write-Host 'Unblocking layout and launcher files...'
Get-ChildItem $layoutDir -Recurse -File | Unblock-File -ErrorAction SilentlyContinue
Get-ChildItem $launcherDir -Recurse -File | Unblock-File -ErrorAction SilentlyContinue

# Portable package: launcher + app folder, uncompressed zip.
Write-Host 'Assembling portable layout...'
$portableZip = Join-Path $dist ($appName + '_' + $greenName + '_v' + $version + '.zip')
New-ZipDirectory -ZipPath $portableZip -SourceDir $layoutDir -NoCompression $true
Write-Host ('Portable package created: ' + $portableZip)

# Installer payload: the very same layout, compressed. It is embedded into the
# installer assembly as a resource (installer\Setup.csproj, EmbeddedResource).
Write-Host 'Building installer payload (installer\payload.zip)...'
New-ZipDirectory -ZipPath $payloadZip -SourceDir $layoutDir -NoCompression $false
if (-not (Test-Path $payloadZip)) { throw 'Payload build failed.' }
Write-Host ('Payload size (bytes): ' + (Get-Item $payloadZip).Length)

# Installer: .NET self-contained single file. No NSIS anywhere in the chain.
Write-Host 'Publishing installer (self-contained single file)...'
New-Item -ItemType Directory -Path $setupPublishDir -Force | Out-Null
$staleSetup = Join-Path $setupPublishDir 'DocHubSetup.exe'
if (Test-Path $staleSetup) { [System.IO.File]::Delete($staleSetup) }
dotnet publish (Join-Path $root 'installer\Setup.csproj') -c Release -r win-x64 --self-contained `
    -p:PublishSingleFile=true -p:EnableCompressionInSingleFile=true -p:DebugType=None -o $setupPublishDir

$setupRaw = Join-Path $setupPublishDir 'DocHubSetup.exe'
if (-not (Test-Path $setupRaw)) {
    $candidate = Get-ChildItem $setupPublishDir -Filter '*.exe' -File | Select-Object -First 1
    if ($candidate) { $setupRaw = $candidate.FullName }
}
if (-not (Test-Path $setupRaw)) { throw 'Installer publish failed: DocHubSetup.exe not found.' }

$setupTarget = Join-Path $dist ($appName + '_' + $setupWord + '_v' + $version + '.exe')
Copy-Item $setupRaw $setupTarget -Force
Write-Host ('Installer created: ' + $setupTarget)

# Clean temporary artifacts (payload.zip must not stay in the repository).
Remove-Item $payloadZip -Force -ErrorAction SilentlyContinue
Remove-Item $build -Recurse -Force -ErrorAction SilentlyContinue

Write-Host ''
Write-Host 'Packaging finished:'
Get-ChildItem $dist | Select-Object Name, Length | Format-Table -AutoSize
