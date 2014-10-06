@echo off
chcp 1251 > nul
set wd=%~dp0
pushd "%wd%"
@REM ~ set PYTHONPATH=
@cls

for %%i in (regingdb.sol.suo regingdb\bin\Release\regingdb.pdb regingdb\obj\Release\regingdb.pdb ^
regingdb\regingdb_TemporaryKey.pfx regingdb\bin\Debug\regingdb.vshost.exe ^
regingdb\bin\Debug\regingdb.vshost.exe.config regingdb\obj\Release\regingdb.exe ^
regingdb\obj\Release\regingdb.application regingdb\obj\Release\Refactor ^
regingdb\obj\Release\regingdb.exe.manifest regingdb\obj\Release\regingdb.TrustInfo.xml) do (
	@echo [%%i]
	del /f /q %%i
)

@REM ~ help del
exit

regingdb\obj\Release\regingdb.csproj.FileListAbsolute.txt
regingdb\bin\Release\regingdb.application
regingdb\obj\Release\regingdb.application
regingdb\bin\Release\regingdb.exe.manifest
regingdb\obj\Release\regingdb.exe.manifest
regingdb\obj\Release\regingdb.TrustInfo.xml
regingdb\bin\Release\regingdb.exe
regingdb\obj\Release\regingdb.exe

regingdb\regingdb.csproj.user
regingdb\regingdb.csproj
regingdb\Properties\app.manifest

regingdb\app.config
regingdb\bin\Release\regingdb.exe.config
regingdb\Program.cs
