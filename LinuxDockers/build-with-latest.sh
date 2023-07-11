set -e

# If empty, grep from expanded_assets
if [[ -z "$1" ]]
then
        echo "Please pass the date arg of the yonixw/mangaprinter:wine8-electron-dotnet48 build"
        exit -1
fi

builddate=$1

echo "Build date docker=$builddate"

docker build --build-arg builddate=$builddate -f 2latest-install.Dockerfile \
	-t yonixw/mangaprinter:mangaprintbase-$builddate .

docker tag yonixw/mangaprinter:mangaprintbase-$builddate yonixw/mangaprinter:latest

docker run --rm -it --shm-size=256m \
	-p 5901:5901 yonixw/mangaprinter:mangaprintbase-$builddate