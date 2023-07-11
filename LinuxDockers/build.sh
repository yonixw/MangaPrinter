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
	-t yonixw/mangaprinter:basegui-$builddate .
docker build --build-arg buildate=$builddate -f 1electron.Dockerfile \
	-t yonixw/mangaprinter:wine8-electron-deps .

#docker run --shm-size=256m -it --rm -p 5901:5901 docker.io/yonixw/mangaprinter:wine8-electron-deps



