FROM  yonixw/mangaprinter:basegui
ENTRYPOINT ["/opt/container_startup.sh"]
EXPOSE 5901
ENV VNC_PASSWD=manga

USER root
COPY ./1install-electron-deps.sh /root
RUN sh /root/1install-electron-deps.sh && rm -f /root/1install-electron-deps.sh 