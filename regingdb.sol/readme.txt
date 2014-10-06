regingdb is a .NET C# console program for one task:
perform the same things as ESRI ArcCatalog do by context menu command
"Register with Geodatabase" for tables in SDE GDB.

Copyright (C) 1996-2010, ALGIS LLC, Valentin Fedulov
Licensed under GNU GENERAL PUBLIC LICENSE (http://www.gnu.org/licenses/gpl.txt)

usage example:

For register table test.table2 in GDB defined by link test.sde,
write CMD file with code like this:

@echo off
set wd=%~dp0
pushd "%wd%"
pushd c:\VisualStudio2008\Projects\regingdb.sol\regingdb\bin\Release

regingdb.exe C:\t\test.sde test.table2

popd
exit
