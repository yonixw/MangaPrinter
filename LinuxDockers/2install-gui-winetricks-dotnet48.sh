# exit when any command fails
set -e
# Run in GUI (required), and then commit the container

winecfg 
winetricks -q dotnet48 
winetricks -q d3dcompiler_47
wine reg add "HKCU\\SOFTWARE\\Microsoft\\Avalon.Graphics" /v DisableHWAcceleration /t REG_DWORD /d 1 /f
winetricks -q unifont