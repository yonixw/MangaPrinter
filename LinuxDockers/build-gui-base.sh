# exit when any command fails
set -e


# Remove 20 from 2020, remove trailing seconds from ISO (we want min only)
#	and then keep only numbers+'T'
export builddate=Y$(date -Iminutes | \
	 sed 's/^[0-9][0-9]//'  | \
	 sed 's/\+00:00//' | \
	 sed 's/[^0-9T]//g')

echo "Build date docker=$builddate"

docker build -f 0novnc.Dockerfile  \
	-t yonixw/mangaprinter:bandi13gui-$builddate .
docker build --build-arg builddate=$builddate -f 1electron.Dockerfile \
	-t yonixw/mangaprinter:wine8-electron-deps-$builddate .

echo "[*] Run and install with guit the /root/2install-gui-winetricks-dotnet48.sh"
(docker stop mp-gui-step && docker rm mp-gui-step) || echo "Skipping delete mp-gui-step..."
docker run --shm-size=256m --name mp-gui-step  -d  \
	-p 5901:5901 yonixw/mangaprinter:wine8-electron-deps-$builddate

echo "[*] Enter any key and then enter"
read -n1

docker commit mp-gui-step yonixw/mangaprinter:wine8-electron-dotnet48-$builddate

