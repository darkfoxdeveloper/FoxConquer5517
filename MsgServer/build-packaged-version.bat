@echo off
@title Build Packaged Exe
dotnet publish -r win-x64 -c Release /p:PublishSingleFile=true /p:PublishTrimmed=true --self-contained --verbosity d
pause