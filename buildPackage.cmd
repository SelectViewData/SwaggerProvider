@echo off
dotnet tool restore
dotnet paket restore
dotnet fake run build.fsx %*
git clean -df
git reset --hard