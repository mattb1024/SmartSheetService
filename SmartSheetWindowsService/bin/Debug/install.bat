

sc create SmartSheetWindowsService4 binpath= "C:\SS\Bin\Debug\SmartSheetWindowsService.exe" start= auto 

sc description SmartSheetWindowsService4 "SmartSheet API Service"

sc failure SmartSheetWindowsService4 reset= 86400 actions= restart/1000/restart/1000/run/1000

sc start SmartSheetWindowsService4
