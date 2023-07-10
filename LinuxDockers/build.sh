# exit when any command fails
set -e

docker build -f 0novnc.Dockerfile  -t yonixw/mangaprinter:basegui .
docker build -f 1electron.Dockerfile -t yonixw/mangaprinter:wine8-electron-deps .

docker run --shm-size=256m -it --rm -p 5901:5901 docker.io/yonixw/mangaprinter:wine8-electron-deps



