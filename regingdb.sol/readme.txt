regingdb is .NET console program for one task:
do the same things as ESRI ArcCatalog do by context menu command
"Register with Geodatabase", I meant SDE GDB.

Copyright (C) 1996-2010, ALGIS LLC
Licensed under GNU GENERAL PUBLIC LICENSE (http://www.gnu.org/licenses/gpl.txt)

usage example: make CMD file with code:

@echo off
set wd=%~dp0
pushd "%wd%"
pushd c:\VisualStudio2008\Projects\regingdb.sol\regingdb\bin\Release
regingdb.exe C:\t\test.sde test.table2
popd
exit
