******************************************************************************
    README - PathMaker Open Source - A VUI Design Tool (Visio 2007/2010 Add-on)
******************************************************************************

This git project contains the following directories:

docs
A single PDF file containing the PathMaker Handbook for reference and learning how to use the VUI design tool

win32_64_installer
A set of files to install the code onto your WindowsXP-SP3 or 7 machines (32 or 64 bit)  (Not tested with Windows 8.x but it should work normally).  
The Setup.exe will install the files necessary to run the tool.  Microsoft Visio 2007/2010, Word 2007/2010 and Excel 2007/2010 are all required for this add-on to function properly.  The tool will work with Visio only, however, the exports of the Word and Excel files will fail if they are not installed.  The COM objects that ship with MS Office are utilized for "interop" functions.  You must install Visio prior to running setup.exe or else it will fail.

vizstudio2010_src
A complete Microsoft Visual Studio 2010 C# project has been included for your use.  The base project name is 
PathMaker and should load into your Studio environment with no issues.

