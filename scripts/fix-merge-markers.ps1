Param()

# Scans files under backend/LensApi for git merge conflict markers and
# resolves them by keeping the first ("ours") section. Creates a .bak
# backup for each modified file.

$root = Join-Path -Path $PSScriptRoot -ChildPath ".." | Resolve-Path -Relative
$path = Join-Path -Path $root -ChildPath "backend/LensApi"

Write-Host "Scanning files under: $path"

$extensions = '*.cs','*.csproj','*.cshtml','*.razor'
$modified = @()

foreach ($ext in $extensions) {
    Get-ChildItem -Path $path -Recurse -Filter $ext -ErrorAction SilentlyContinue | ForEach-Object {
        $file = $_.FullName
        $lines = Get-Content -LiteralPath $file -Encoding UTF8 -Raw -ErrorAction Stop | Out-String
        if ($lines -match '<<<<<<<') {
            Write-Host "Found markers in: $file"
            $arr = Get-Content -LiteralPath $file -Encoding UTF8
            $out = New-Object System.Collections.Generic.List[System.String]
            $i = 0
            while ($i -lt $arr.Count) {
                $line = $arr[$i]
                if ($line -match '^<<<<<<<') {
                    # skip marker
                    $i++
                    $ours = @()
                    while ($i -lt $arr.Count -and -not ($arr[$i] -match '^=======$')) {
                        $ours += $arr[$i]
                        $i++
                    }
                    # skip =======
                    if ($i -lt $arr.Count -and $arr[$i] -match '^=======$') { $i++ }
                    # skip theirs until >>>>>>>
                    while ($i -lt $arr.Count -and -not ($arr[$i] -match '^>>>>>>>')) { $i++ }
                    if ($i -lt $arr.Count -and $arr[$i] -match '^>>>>>>>') { $i++ }
                    # append ours
                    foreach ($l in $ours) { $out.Add($l) }
                } else {
                    $out.Add($line)
                    $i++
                }
            }
            # backup and write
            Copy-Item -LiteralPath $file -Destination ($file + '.bak') -Force
            $out | Set-Content -LiteralPath $file -Encoding UTF8
            $modified += $file
        }
    }
}

if ($modified.Count -eq 0) {
    Write-Host "No files modified."
    exit 0
}

Write-Host "Modified files:`n" + ($modified -join "`n")
exit 0
