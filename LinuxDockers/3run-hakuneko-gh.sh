mkdir -p ~/Downloads
cd ~/Downloads

ZIP_PATH=$(curl -L https://github.com/manga-download/hakuneko/releases/latest | grep -oE '[^"]+\/releases\/[^"]+\.deb' | grep "amd64" )
EXPAND_PATH=$(curl -L https://github.com/manga-download/hakuneko/releases/latest | grep -oE '[^"]+\/expanded_assets\/[^"]+' )

echo "EXPAND_PATH=$EXPAND_PATH"

# If empty, grep from expanded_assets
if [[ -z $ZIP_PATH ]]
then
        ZIP_PATH=$(curl $EXPAND_PATH  | grep -oE '[^"]+release[^"]+\.deb' | grep "amd64")
fi

echo "ZIP_PATH=$ZIP_PATH"


if [[ -z $ZIP_PATH ]]
then
        echo "Can't find latest release from MangaPrinter, exiting..."
        exit -1
fi

if [[ ! $ZIP_PATH = http* ]]
then
        echo "Adding https github to link"
        ZIP_PATH=https://github.com$ZIP_PATH
fi

set -e # exit on any fail

wget -q -O HakuNeko_latest.deb $ZIP_PATH

dpkg -i HakuNeko_latest.deb

export QT_X11_NO_MITSHM=1
/usr/lib/hakuneko-desktop/hakuneko \
	--no-sandbox --enable-logging \
	--disable-gpu  --in-process-gpu --disable-software-rasterizer \
	--no-xshm --disable-dev-shm-usage

