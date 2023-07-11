mkdir -p ~/Downloads
cd ~/Downloads

ZIP_PATH=$(curl -L https://github.com/yonixw/MangaPrinter/releases/latest | grep -oE '[^"]+\/releases\/[^"]+\.zip' )
EXPAND_PATH=$(curl -L https://github.com/yonixw/MangaPrinter/releases/latest | grep -oE '[^"]+\/expanded_assets\/[^"]+' )

echo "EXPAND_PATH=$EXPAND_PATH"

# If empty, grep from expanded_assets
if [[ -z $ZIP_PATH ]]
then
	ZIP_PATH=$(curl $EXPAND_PATH  | grep -oE '[^"]+release[^"]+\.zip')
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

wget -q -O MangaPrinter_latest.zip $ZIP_PATH

unzip -o MangaPrinter_latest.zip

find . | grep -i "mangaprinter" | grep -E "\.exe$" | xargs wine
