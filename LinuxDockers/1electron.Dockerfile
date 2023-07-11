ARG builddate

FROM  yonixw/mangaprinter:basegui-$buildate
ENTRYPOINT ["/opt/container_startup.sh"]
EXPOSE 5901
ENV VNC_PASSWD=manga

USER root
RUN mkdir -p /root
COPY ./1install-wine-electron-deps.sh /root/
RUN sh /root/1install-wine-electron-deps.sh && rm -f /root/1install-wine-electron-deps.sh

# Prepare for manual GUI running:
COPY ./2install-gui-winetricks-dotnet48.sh /root/
