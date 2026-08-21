@echo OFF
echo Stopping old service version...
net stop "PipeHistoryLocalService"
echo Uninstalling old service version...
sc delete "PipeHistoryLocalService"

echo Installing service...
rem DO NOT remove the space after "binpath="!
sc create "PipeHistoryLocalService" binpath= "C:\\Work\\UnE\\src\\KPX\\PipeHistoryLocalService\\PipeHistoryLocalService\\bin\\x86\\Release\\PipeHistoryLocalService.exe" start= auto
echo Starting server complete
pause