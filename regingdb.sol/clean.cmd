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
