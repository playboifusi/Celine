param (
    [string]$dllPath,
    [string]$hashFilePath
)

$hash = Get-FileHash $dllPath -Algorithm SHA256
$hash.Hash | Out-File $hashFilePath
