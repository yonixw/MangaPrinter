docker build -f 0novnc.Dockerfile  -t yonixw/mangaprinter:basegui .
docker run --shm-size=256m -it --rm -p 5901:5901 yonixw/mangaprinter:basegui



