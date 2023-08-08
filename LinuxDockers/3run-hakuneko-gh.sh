mkdir -p ~/Downloads
cd ~/Downloads

#######################################################

#!/usr/bin/env bash
# from: https://github.com/MuhammedKalkan/OpenLens/issues/130

GH_DL_REPO=manga-download/hakuneko
# WARN: double check no weird stuff with $GH_DL_FILTER
GH_DL_SAVEPATH=HakuNeko_latest.deb
GH_DL_FILTER=amd64.deb

set -e

LATEST_URL=$(curl https://api.github.com/repos/$GH_DL_REPO/releases/latest | 
    jq -r '.assets[] |  .browser_download_url' | grep -i "$GH_DL_FILTER")

echo "LATEST_URL=$LATEST_URL"


if [[ -z $LATEST_URL ]]
then
 echo "Couldn't get latest link, exiting."
 exit 1
fi

curl -L $LATEST_URL > "$GH_DL_SAVEPATH"

#######################################################

set +e

dpkg -i "$GH_DL_SAVEPATH"

export QT_X11_NO_MITSHM=1
/usr/lib/hakuneko-desktop/hakuneko \
	--no-sandbox --enable-logging \
	--disable-gpu  --in-process-gpu --disable-software-rasterizer \
	--no-xshm --disable-dev-shm-usage

sleep 3