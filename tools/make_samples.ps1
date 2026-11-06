$ErrorActionPreference = 'Stop'
$root = Join-Path $PSScriptRoot 'samples'
if (Test-Path $root) { Remove-Item $root -Recurse -Force }
New-Item -ItemType Directory -Path $root | Out-Null

$word = New-Object -ComObject Word.Application
$word.Visible = $false
$word.DisplayAlerts = 0

function Add-Text($doc, $text) {
  $r = $doc.Content
  $r.Collapse(0) | Out-Null
  $r.InsertAfter($text)
}

function Add-Break($doc, $type) {
  $r = $doc.Content
  $r.Collapse(0) | Out-Null
  $r.InsertBreak($type)
}

function Set-Paper($doc, $paper, $orientation) {
  foreach ($section in $doc.Sections) {
    $section.PageSetup.PaperSize = $paper
    $section.PageSetup.Orientation = $orientation
  }
}

function Add-Footer($doc) {
  foreach ($section in $doc.Sections) {
    $footer = $section.Footers.Item(1)
    $footer.Range.Text = 'Page '
    $r = $footer.Range
    $r.Collapse(0) | Out-Null
    $doc.Fields.Add($r, 33, [Type]::Missing, $false) | Out-Null
  }
}

function Save-All($doc, $baseName) {
  $doc.SaveAs2((Join-Path $root ($baseName + '.docx')), 16)
  $doc.ExportAsFixedFormat((Join-Path $root ($baseName + '.pdf')), 17)
}

# 1) A4 portrait, 2 pages, table and footer page numbers
$d = $word.Documents.Add()
Set-Paper $d 7 0
$d.Content.Font.Name = 'Microsoft YaHei'
$d.Content.Font.Size = 12
Add-Text $d ('DOC-A1  A4 PORTRAIT  PAGE-1' + [char]13)
Add-Text $d ('This is the first test document.' + [char]13)
$t = $d.Tables.Add($d.Range(0, 0), 3, 3)
$t.Borders.Enable = $true
for ($i = 1; $i -le 3; $i++) { for ($j = 1; $j -le 3; $j++) { $t.Cell($i, $j).Range.Text = "A-$i-$j" } }
Add-Break $d 7
Add-Text $d ('DOC-A1  PAGE-2' + [char]13 + 'Second page of A4 portrait document.' + [char]13)
Add-Footer $d
Save-All $d '01_A4_portrait'
$d.Close(0)

# 2) Legacy .doc, A3 landscape, 2 pages
$d = $word.Documents.Add()
Set-Paper $d 6 1
$d.Content.Font.Name = 'Microsoft YaHei'
$d.Content.Font.Size = 14
Add-Text $d ('DOC-B1  A3 LANDSCAPE  PAGE-1' + [char]13 + 'Legacy .doc file, wide page.' + [char]13)
Add-Break $d 7
Add-Text $d ('DOC-B1  PAGE-2' + [char]13 + 'Second landscape page.' + [char]13)
$d.SaveAs2((Join-Path $root '02_A3_landscape.doc'), 0)
$d.ExportAsFixedFormat((Join-Path $root '02_A3_landscape.pdf'), 17)
$d.Close(0)

# 3) Letter with three sections: portrait, landscape, portrait
$d = $word.Documents.Add()
$d.Content.Font.Name = 'Microsoft YaHei'
$d.Sections.Item(1).PageSetup.PaperSize = 2
$d.Sections.Item(1).PageSetup.Orientation = 0
Add-Text $d ('DOC-C1  LETTER PORTRAIT  PAGE-1' + [char]13 + 'Section one.' + [char]13)
Add-Break $d 2
$d.Sections.Item(2).PageSetup.PaperSize = 2
$d.Sections.Item(2).PageSetup.Orientation = 1
Add-Text $d ('DOC-C1  LETTER LANDSCAPE  PAGE-2' + [char]13 + 'Section two is landscape.' + [char]13)
Add-Break $d 2
$d.Sections.Item(3).PageSetup.PaperSize = 2
$d.Sections.Item(3).PageSetup.Orientation = 0
Add-Text $d ('DOC-C1  LETTER PORTRAIT  PAGE-3' + [char]13 + 'Section three is portrait again.' + [char]13)
Add-Footer $d
Save-All $d '03_Letter_mixed'
$d.Close(0)

# 4) RTF input
$d = $word.Documents.Add()
$d.Content.Font.Name = 'Microsoft YaHei'
Add-Text $d ('DOC-D1  RTF  PAGE-1' + [char]13 + 'Rich text format input.' + [char]13)
Add-Footer $d
$d.SaveAs2((Join-Path $root '04_rtf.rtf'), 6)
$d.ExportAsFixedFormat((Join-Path $root '04_rtf.pdf'), 17)
$d.Close(0)

# 5) Pure PDF input, A5 landscape, one page
$d = $word.Documents.Add()
Set-Paper $d 11 1
$d.Content.Font.Name = 'Microsoft YaHei'
Add-Text $d ('DOC-E1  A5 LANDSCAPE  PAGE-1' + [char]13 + 'Standalone PDF input.' + [char]13)
$d.ExportAsFixedFormat((Join-Path $root '05_A5_input.pdf'), 17)
$d.Close(0)

$word.Quit()
Get-ChildItem $root | Select-Object Name, Length | Format-Table -AutoSize
