

sc create SmartSheetWindowsService5 binpath= "C:\Users\mb001\Documents\Visual Studio 2013\Service Builds\1\SmartSheetWindowsSvc v1.0\Source\SmartSheetWindowsService\bin\Release\SmartSheetWindowsService.exe" start= auto 

sc description SmartSheetWindowsService5 "SmartSheet API Service"

SC failure SmartSheetWindowsService5 reset= 86400 actions= restart/1000/restart/1000/run/1000

