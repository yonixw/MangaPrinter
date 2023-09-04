mkdir -p ~/Downloads
cd ~/Downloads


#######################################################

#!/usr/bin/env bash
# from: https://github.com/MuhammedKalkan/OpenLens/issues/130

GH_DL_REPO=yonixw/MangaPrinter
# WARN: double check no weird stuff with $GH_DL_FILTER
GH_DL_SAVEPATH=MangaPrinter_latest_64.zip
GH_DL_FILTER=.zip

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

# windows zip slashes cause fail, so we will try anyway :/
unzip -o "$GH_DL_SAVEPATH" || (echo "Unzip problem.. trying anyway.." && sleep 3)

find . | grep -i "mangaprinter" | grep -E "\.exe$" | xargs wine64

sleep 3