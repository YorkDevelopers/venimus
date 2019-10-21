$ErrorActionPreference = "Stop"

New-Item -Type Directory "./downloads" -ErrorAction SilentlyContinue

$mdbVersion = "2012plus-4.2.1"

[Net.ServicePointManager]::SecurityProtocol = "tls12, tls11, tls"
$url = "https://fastdl.mongodb.org/win32/mongodb-win32-x86_64-$mdbVersion.zip"
$output = "./downloads/mongodb-win32-x86_64-$mdbVersion.zip"
Invoke-WebRequest -Uri $url -OutFile $output

Expand-Archive $output -DestinationPath "./downloads/mongodb-win32-x86_64-$mdbVersion"

New-Item -Type Directory "/data/db" -ErrorAction SilentlyContinue

Start-Process "./downloads/mongodb-win32-x86_64-$mdbVersion/mongodb-win32-x86_64-$mdbVersion/bin/mongod.exe"
