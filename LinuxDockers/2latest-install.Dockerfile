ARG builddate

FROM  yonixw/mangaprinter:wine8-electron-dotnet48-${builddate}
ENTRYPOINT ["/opt/container_startup.sh"]
EXPOSE 5901
ENV VNC_PASSWD=manga

USER root
RUN mkdir -p /root && rm -f /root/*.sh
COPY ./3run-hakuneko-gh.sh /root/run-hakuneko-gh.sh
COPY ./4run-mangaprinter-gh.sh /root/run-mangaprinter-gh.sh

# Change Xterm font size // https://superuser.com/a/1722981/273364
RUN echo "?package(bash):needs=\"X11\" section=\"DockerCustom\" title=\"HakuNeko\" command=\"xterm -ls -bg black -fg white -fs 14 -fa DejaVuSansMono -e \\\"bash /root/run-hakuneko-gh.sh\\\" \"" \
    >> /usr/share/menu/custom-docker && update-menus &&\
    echo "?package(bash):needs=\"X11\" section=\"DockerCustom\" title=\"MangaPrinter\" command=\"xterm -ls -bg black -fg white -fs 14 -fa DejaVuSansMono -e \\\"bash /root/run-mangaprinter-gh.sh\\\" \"" \
    >> /usr/share/menu/custom-docker && update-menus