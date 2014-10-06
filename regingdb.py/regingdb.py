# -*- coding: utf-8 -*-

'''
Created on 07.12.2010

@author: Valik

Register with Geodatabase program.

python_arcobjects.pdf    - презентация использования AO в Python (http://www.pierssen.com/arcgis/misc.htm)
python_arcobjects.zip    - код примеров с (http://www.pierssen.com/arcgis/misc.htm), сниппеты оттуда
regingdb.py              - сама прога на Питон
snippets.py              - сниппеты, используются в проге
'''

import sys
from snippets import GetLibPath, NewObj, CType

ecErr = 1
ecOK = 0

def getModule(sModuleName):
    """Import ArcGIS module"""
    from comtypes.client import GetModule
    sLibPath = GetLibPath()
    GetModule(sLibPath + sModuleName)

getModule("esriSystem.olb")
getModule("esriGeoDatabase.olb")
getModule("esriDataSourcesGDB.olb")

import comtypes.gen.esriSystem as esriSystem
import comtypes.gen.esriGeoDatabase as esriGeoDatabase
import comtypes.gen.esriDataSourcesGDB as esriDataSourcesGDB


def initProductLic():
    """Init standalone ArcGIS license"""
    pInit = NewObj(esriSystem.AoInitialize,
                   esriSystem.IAoInitialize)
    ProductList = [esriSystem.esriLicenseProductCodeArcServer,
                   esriSystem.esriLicenseProductCodeArcView]
    for eProduct in ProductList:
        licenseStatus = pInit.IsProductCodeAvailable(eProduct)
        if licenseStatus != esriSystem.esriLicenseAvailable:
            continue
        licenseStatus = pInit.Initialize(eProduct)
        return (licenseStatus == esriSystem.esriLicenseCheckedOut)
    return False


def doWork(sdeconnfile='C:/sde/rngis.rgo.sde', tabname='RGO.ADMIN_A', oidfieldname='OBJECTID'):
    res = initProductLic()
    if res: print 'licence OK'
    else:
        print 'no licence'
        return ecErr

    # IWorkspaceFactory wspf = new SdeWorkspaceFactoryClass();
    wspf = NewObj(esriDataSourcesGDB.SdeWorkspaceFactory,
                  esriGeoDatabase.IWorkspaceFactory)
    ''' @type wspf: esriGeoDatabase.IWorkspaceFactory '''

    wsp = wspf.OpenFromFile(sdeconnfile, 0)
    # IFeatureWorkspaceManage fwspm = (IFeatureWorkspaceManage)wsp;
    fwspm = CType(wsp, esriGeoDatabase.IFeatureWorkspaceManage)
    ''' @type fwspm: esriGeoDatabase.IFeatureWorkspaceManage '''

    if fwspm.IsRegisteredAsObjectClass(tabname):
        print 'registered already [%s]' % tabname
        return ecOK

    fwsp = CType(wsp, esriGeoDatabase.IFeatureWorkspace)
    tbl = fwsp.OpenTable(tabname)
    oc = CType(tbl, esriGeoDatabase.IObjectClass)
    return registerWithGeodatabase(oc, oidfieldname)


def registerWithGeodatabase(objclass = '', oidfieldname = ''):
    if not oidfieldname: oidfieldname = 'OBJECTID'
    schemaLock = CType(objclass, esriGeoDatabase.ISchemaLock)
    try:
        schemaLock.ChangeSchemaLock(esriGeoDatabase.esriSchemaLock.esriExclusiveSchemaLock)
        classSchemaEdit = CType(objclass, esriGeoDatabase.IClassSchemaEdit)
        classSchemaEdit.RegisterAsObjectClass(oidfieldname, '')
        return ecOK
    finally:
        schemaLock.ChangeSchemaLock(esriGeoDatabase.esriSchemaLock.esriSharedSchemaLock)
    return ecErr


if __name__ == '__main__':
    argc = len(sys.argv)
    res = ecErr
    print 'argc: [%s], argv: [%s]' % (argc, sys.argv)
    if argc < 3:
        print 'usage example: python.exe regingdb.py C:/rngis.sde RGO.TABLE1 OBJECTID'
        sys.exit(res)
    oidfname = 'OBJECTID'
    if argc > 3: oidfname=sys.argv[3]

    try:
        res = doWork(sdeconnfile=sys.argv[1], tabname=sys.argv[2], oidfieldname=oidfname)
        print 'done'
    except Exception, e:
        if type(e).__name__ == 'COMError':
            print 'COM Error, msg [%s]' % e
        else:
            print 'Error, msg [%s]' % e.Message
    sys.exit(res)
