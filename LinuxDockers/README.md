# Info

This docker comes pre installed with wine (version 8), dotnet48 (from winetricks) and electron dependencies.
It is ready for MangaPrinter (this Windows WPF App) and HakuNeko (Electron App).

The docker uses a X11 server and gives access to the desktop using a web UI based on `noVNC`.

# How to run

Running requires docker. 

Assuming you want the downloaded files to stay persistent in the `Mangas` folder:

1. Run

```
docker run --rm -it --shm-size=256m -v "$(pwd)/Mangas:/root/Mangas" -p 5901:5901 yonixw/mangaprinter
```

2. Open your browser and go to the desktop web interface at `http://localhost:5901`
3. Enter the default password: `manga` (can be changed with environment `VNC_PASSWD`)
4. Right click on the black desktop for available programs. Look under the `DockerCustom` menu.


# Limitations

The latest docker image size is 6GB after download (2GB download)

You cannot run this docker image on a limited environment (like non sudo environments). Mainly because wine 8 will not work. For example, it cannot be used in [Gitpod.io](https://www.gitpod.io/) which gives you a limited dev environment.

# Links 

Link to the base gui in docker repo:
* https://github.com/bandi13/gui-docker

Link to this repo:
* https://github.com/yonixw/MangaPrinter/

Link to dockerfiles:
* https://github.com/yonixw/MangaPrinter/tree/master/LinuxDockers
