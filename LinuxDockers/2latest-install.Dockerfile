ARG builddate

FROM  yonixw/mangaprinter:wine8-electron-dotnet48-${builddate}
ENTRYPOINT ["/opt/container_startup.sh"]
EXPOSE 5901
ENV VNC_PASSWD=manga

USER root
RUN mkdir -p /root && rm -f /root/*.sh && \
    apt update && apt install -y detox jq && \
    apt clean
COPY ./3run-hakuneko-gh.sh /root/run-hakuneko-gh.sh
COPY ./4run-mangaprinter-gh.sh /root/run-mangaprinter-gh.sh
COPY ./4run-mangaprinter-gh-64.sh /root/run-mangaprinter-gh-64.sh
COPY ./5run-firefox.sh /root/run-firefox.sh

# Change Xterm font size // https://superuser.com/a/1722981/273364
RUN echo "?package(bash):needs=\"X11\" section=\"DockerCustom\" title=\"01 HakuNeko\" command=\"xterm -ls -bg black -fg white -fs 14 -fa DejaVuSansMono -e \\\"bash /root/run-hakuneko-gh.sh\\\" \"" \
    >> /usr/share/menu/custom-docker && update-menus &&\
    echo "?package(bash):needs=\"X11\" section=\"DockerCustom\" title=\"03 MangaPrinter (32bit)\" command=\"xterm -ls -bg black -fg white -fs 14 -fa DejaVuSansMono -e \\\"bash /root/run-mangaprinter-gh.sh\\\" \"" \
    >> /usr/share/menu/custom-docker && update-menus &&\
    echo "?package(bash):needs=\"X11\" section=\"DockerCustom\" title=\"03 MangaPrinter (64bit)\" command=\"xterm -ls -bg black -fg white -fs 14 -fa DejaVuSansMono -e \\\"bash /root/run-mangaprinter-gh-64.sh\\\" \"" \
    >> /usr/share/menu/custom-docker && update-menus &&\
    echo "?package(bash):needs=\"X11\" section=\"DockerCustom\" title=\"02 Fix Names\" command=\"xterm -ls -bg black -fg white -fs 14 -fa DejaVuSansMono -e \\\"detox -r /root/Mangas/\\\" \"" \
    >> /usr/share/menu/custom-docker && update-menus &&\
    echo "?package(bash):needs=\"X11\" section=\"DockerCustom\" title=\"04 Firefox\" command=\"xterm -ls -bg black -fg white -fs 14 -fa DejaVuSansMono -e \\\"bash /root/run-firefox.sh\\\" \"" \
    >> /usr/share/menu/custom-docker && update-menus

