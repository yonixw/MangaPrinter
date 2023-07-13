# exit when any command fails
set -e

apt update && \
    apt -y install libgtkextra-dev libgconf2-dev \
                   libnss3 libasound2 libxtst-dev libxss1
apt install -y wget curl lsb-release

# Install wine for x64+x86
dpkg --add-architecture i386
mkdir -p /etc/apt/keyrings
wget -O /etc/apt/keyrings/winehq-archive.key https://dl.winehq.org/wine-builds/winehq.key
# lsb_release -sc
wget -NP /etc/apt/sources.list.d/ https://dl.winehq.org/wine-builds/ubuntu/dists/$(lsb_release -sc)/winehq-$(lsb_release -sc).sources
apt update
apt -y install --install-recommends winehq-stable

# Install winetricks
apt install -y cabextract p7zip unrar unzip wget zenity
curl --silent --show-error https://raw.githubusercontent.com/Winetricks/winetricks/master/src/winetricks --stderr - | grep ^WINETRICKS_VERSION | cut -d '=' -f 2
wget https://raw.githubusercontent.com/Winetricks/winetricks/master/src/winetricks
chmod +x winetricks 
mv -v winetricks /usr/bin/

echo ttf-mscorefonts-installer msttcorefonts/accepted-mscorefonts-eula select true | debconf-set-selections
apt-get -y --reinstall install ttf-mscorefonts-installer

winetricks --version

# clean
apt clean all