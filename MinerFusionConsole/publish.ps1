$WindowsPath = $args[0] + "\Windows"
$LinuxPath = $args[0] + "\Linux"
$MacOsPath = $args[0] + "\MacOs"

$ApplicationVersion = "v0.2-beta"
$BaseArchiveName = "MinerFusionConsole-" + $ApplicationVersion

$WindowsArchive = $BaseArchiveName + "-Win64" + ".zip"
$LinuxArchive = $BaseArchiveName + "-linux64" + ".tar.gz"
$MacOsArchive = $BaseArchiveName + "-MacOS64" + ".tar.gz"

Write-Host "Publishing for Windows"
dotnet publish -o $WindowsPath --configuration Release --runtime win-x64 --self-contained false -p:PublishSingleFile=True

Write-Host "Publishing for Linux"
dotnet publish -o $LinuxPath --configuration Release --runtime linux-x64 --self-contained false -p:PublishSingleFile=True

Write-Host "Publishing for MacOS"
dotnet publish -o $MacOsPath --configuration Release --runtime osx-x64 --self-contained false -p:PublishSingleFile=True

Write-Host "Removing debugging symbols"
$CurrentLocation = Get-Location
Set-Location $args[0]
Get-ChildItem * -Include *.pdb -Recurse | Remove-Item

Write-Host "Archiving Binaries"

Compress-Archive -Path $WindowsPath -DestinationPath $WindowsArchive
tar -cvzf $LinuxArchive Linux
tar -cvzf $MacOsArchive MacOs

Write-Host "Calculating MD5 sums for archives"
CertUtil -hashfile $WindowsArchive MD5
CertUtil -hashfile $LinuxArchive MD5
CertUtil -hashfile $MacOsArchive MD5

Set-Location $CurrentLocation

