# Info

This docker comes pre installed with wine (version 8) for MangaPrinter (Windows WPF App) and HakuNeko (Electron App).

This docker uses a X11 server and gives access to the desktop using a web UI based on `noVNC`.

# How to run

Assuming you want the downloaded files to stay persistent:

```
docker run --rm -it --shm-size=256m -v "$(pwd)/Mangas:/root/Mangas" -p 5901:5901 yonixw/mangaprinter
```

Access the desktop at `http://localhost:5901`, and right click on it for available programs. The relevant manga apps are under the `DockerCustom` menu.

 Default password is "manga" and can be changed with environment `VNC_PASSWD`.

# Limitations

You cannot run this docker on a limited environment (like docker in docker and not privileged). Mainly because wine 8 will not work. For example, it cannot be used in [Gitpod.io](https://www.gitpod.io/) which gives you a limited dev environment.

# Links 

Link to the base gui in docker repo:
* https://github.com/bandi13/gui-docker

Link to this repo:
* https://github.com/yonixw/MangaPrinter/

Link to dockerfiles:
* https://github.com/yonixw/MangaPrinter/tree/master/LinuxDockers
