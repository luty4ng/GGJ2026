@echo off
:: 检测是否有管理员权限
net session >nul 2>&1
if %errorlevel% neq 0 (
    echo Requesting administrator privileges...
    powershell -Command "Start-Process '%~f0' -Verb RunAs"
    exit /b
)

mklink /D "D:\Luty\Unity Projects\ProjectAE\AnchoredExpanse\Assets\AssetArt\Configs\SO_Configs" "D:\Luty\Unity Projects\ProjectAE\Configs\GameConfig\Datas\SO_Configs" 