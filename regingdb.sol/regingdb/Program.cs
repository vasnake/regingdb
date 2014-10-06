/*
regingdb is .NET console program for one task:
do the same things as ESRI ArcCatalog do by context menu command
"Register with Geodatabase", I meant SDE GDB.

Copyright (C) 1996-2010, ALGIS LLC
Originally by Valik <vasnake@gmail.com>, 2010.

    This file is part of Regingdb.

    Regingdb is free software: you can redistribute it and/or modify
    it under the terms of the GNU General Public License as published by
    the Free Software Foundation, either version 3 of the License, or
    (at your option) any later version.

    Regingdb is distributed in the hope that it will be useful,
    but WITHOUT ANY WARRANTY; without even the implied warranty of
    MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
    GNU General Public License for more details.

    You should have received a copy of the GNU General Public License
    along with Regingdb.  If not, see <http://www.gnu.org/licenses/>.
*/

using System;
using System.Runtime.InteropServices;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;
using ESRI.ArcGIS.esriSystem;
using ESRI.ArcGIS.Geodatabase;
using ESRI.ArcGIS.DataSourcesGDB;
//using ESRI.ArcGIS.Framework;
//using ESRI.ArcGIS.ADF;

namespace regingdb {
    public enum progResultCode : int {
        good = 0,
        bad = 1,
        unknown = 2
    }

    /// <summary>
    /// Logger, printout messages for user
    /// </summary>
    class Log {
        public static int level = 1;

        public static void p(string str, int dbgLev, string dst) {
            if (level < dbgLev) {
                return;
            }
            if (dst == "err" || dst == "both") {
                System.Console.Error.Write(str + "\n");
            }
            if (dst == "both" || dst == "log") {
                System.Console.Out.Write(str + "\n");
            }
        }
        public static void p(string str, int dbgLev) {
            p(str, dbgLev, "err");
        }
        public static void p(string str, string dst) {
            p(str, 1, dst);
        }
        public static void p(string str) {
            p(str, 1, "err");
        }

        public static void rp(string str) {
            System.Console.Write(str);
        }
    } // class Log


    /// <summary>
    /// Task processor for "Register in Geodatabase"
    /// usage:
    ///     // RegInEsriGDB mTask = new RegInEsriGDB(sde, tab, fld);
    ///        mTask.initLic();
    ///        mTask.doWork();
    ///     // finally: mTask.shutdown();
    /// </summary>
    class RegInEsriGDB {
        public IAoInitialize
            mLicInit = null;
        public String
            mSdeConnFileName = "", mTabName = "", mOidFldName = "";

        public
            RegInEsriGDB(String sdeConnFileName, String tabName, String oidFieldName) {
            //Log.p("RegInEsriGDB constructor");
            mSdeConnFileName = sdeConnFileName;
            mTabName = tabName;
            mOidFldName = oidFieldName;
            //throw (new Exception("oops, get TODO?"));
        } // constructor


        public void
            initLic() {
            try {
                IAoInitialize ini = new AoInitializeClass();
                mLicInit = ini;
                esriLicenseProductCode pCode =
                    esriLicenseProductCode.esriLicenseProductCodeArcServer;
                // work on cli: esriLicenseProductCode.esriLicenseProductCodeArcView;
                // work on srv: esriLicenseProductCode.esriLicenseProductCodeArcServer;
                esriLicenseStatus licstat = ini.Initialize(pCode);
                Log.p("Lic.stat is [" + licstat + "]");
                if (licstat == esriLicenseStatus.esriLicenseAlreadyInitialized ||
                    licstat == esriLicenseStatus.esriLicenseAvailable ||
                    licstat == esriLicenseStatus.esriLicenseCheckedOut) {
                    //good
                    Log.p("Lic.available");
                }
                else {
                    //bad
                    Log.p("Lic.not available, try another");
                    pCode = esriLicenseProductCode.esriLicenseProductCodeArcView;
                    licstat = ini.Initialize(pCode);
                    Log.p("Lic.stat2 is [" + licstat + "]");
                }
                if (ini.InitializedProduct() == pCode) {
                    Log.p("OK, have good lic.");
                }
                else {
                    Log.p("prod.code is [" + pCode + "] but inited prod.code is [" +
                        ini.InitializedProduct() + "]");
                }
            }
            catch (Exception e) {
                Log.p("ERR, initLic exception: " + e.Message);
                throw e;
            }
        } // method initLic


        public void shutdown() {
            if (mLicInit != null) mLicInit.Shutdown();
        }


        public void
            doWork() {
            String sdeconnfname = mSdeConnFileName; // "c:\\t\\test.sde";
            String tabname = mTabName; // "TEST.TABLE1";

            Log.p("doWork started...");
            IWorkspaceFactory wspf = new SdeWorkspaceFactoryClass();
            Log.p("open worksp. from sde conn.file [" + sdeconnfname + "]");
            IWorkspace wsp = wspf.OpenFromFile(sdeconnfname, 0);

            IFeatureWorkspaceManage fwspm = (IFeatureWorkspaceManage)wsp;
            Boolean isreg = fwspm.IsRegisteredAsObjectClass(tabname);
            if (isreg != false) {
                Log.p("registered already, tab.name [" + tabname + "]", "both");
//                throw (new Exception("registered already, tab.name [" + tabname + "]"));
                return;
            }

            IFeatureWorkspace fwsp = (IFeatureWorkspace)wsp;
            Log.p("open tab. by name [" + tabname + "]", "both");
            ITable tbl = fwsp.OpenTable(tabname);
            // IObjectClass from ITable
            // IClassSchemaEdit from objectClass
            // ISchemaLock from objectClass
            // schemaLock.ChangeSchemaLock()
            // schemaEdit.RegisterAsObjectClass()
            // schemaLock.ChangeSchemaLock()
            IObjectClass oc = (IObjectClass)tbl;
            Log.p("do register...");
            RegisterWithGeodatabase(oc, mOidFldName);

            Log.p("OK, registered.", "both");
        } // method doWork


        protected void
            RegisterWithGeodatabase(IObjectClass objectClass, String oidFieldName) {
            if (oidFieldName == "") {
                oidFieldName = "OBJECTID";
            }
            // Attempt to acquire an exclusive schema lock for the object class.
            ISchemaLock schemaLock = (ISchemaLock)objectClass;
            try {
                schemaLock.ChangeSchemaLock(esriSchemaLock.esriExclusiveSchemaLock);
                // If this point is reached, the exclusive lock was acquired. We can cast the object
                // class to IClassSchemaEdit and call RegisterAsObjectClass.
                IClassSchemaEdit classSchemaEdit = (IClassSchemaEdit)objectClass;
                classSchemaEdit.RegisterAsObjectClass(oidFieldName, "");
            }
            catch (COMException comExc) {
                // Re-throw the exception.
                throw comExc;
            }
            finally {
                // Reset the lock on the object class to a shared lock.
                schemaLock.ChangeSchemaLock(esriSchemaLock.esriSharedSchemaLock);
            }
        } // method RegisterWithGeodatabase

    } // class RegInEsriGDB


    /// <summary>
    /// Main program, do "Register in Geodatabase" for SDE table
    /// </summary>
    class Program {
        public static RegInEsriGDB mTask = null;

        /// <summary>
        /// Params for program: sdeConnect filename, tableName, OID fieldName (optional)
        /// </summary>
        /// <param name="args"></param>
        static void parseArgs(string[] args) {
            if (args.Length < 2) {
                Log.p("args.len [" + args.Length + "]");
                throw (new Exception(
                    "usage example: regingdb.exe c:\\t\\sdc.sde TEST.TABLE1 OBJECTID"));
            }
            String sde = args[0], tab = args[1], fld = "";
            if (args.Length >= 3) {
                fld = args[2];
            }
            Log.p("get params, sdeConnFname [" + sde
                + "], tabName [" + tab
                + "], oidFldName [" + fld + "]");
            RegInEsriGDB p = new RegInEsriGDB(sde, tab, fld);
            mTask = p;
        } // method parseArgs


        static void Main(string[] args) {
            Log.level = 3;
            Log.p("Reg.in GDB program working...");
            Environment.ExitCode = (int)progResultCode.good;
            try {
                parseArgs(args);
                //RegInEsriGDB mTask = new RegInEsriGDB(sde, tab, fld);
                mTask.initLic();
                mTask.doWork();
                //finally: mTask.shutdown();
            }
            catch (COMException e) {
                Log.p("COM Error", "both"); Log.p(e.Message);
                Environment.ExitCode = (int)progResultCode.bad;
            }
            catch (Exception e) {
                Log.p("Error", "both"); Log.p(e.Message);
                Environment.ExitCode = (int)progResultCode.bad;
            }
            finally {
                Log.p("Program done.");
                if (mTask != null) mTask.shutdown();
            }
        } // method Main

    } // class Program
} // namespace regingdb
