# Manga Printer

The tool that will let you print your manga/comics images

## The problem:

Downloading manga from the internet (**Legally of course, like from [MANGA Plus by SHUEISHA](https://mangaplus.shueisha.co.jp/)**) can result in complex situations:
* You have some files with double panels and some with a single panel. 
* Simply cutting every double page into 2 files of singles will not work because you need to consider whether the chapter is being read from right to left or vice versa. 
* Also, since the chapter was stripped from any advertisement pages in between, some doubles will be split across printed pages if not treated correctly.

## The solution:

Here is where MangaPrinter comes to help. It will let you:


* Manage both chapters and pages
* Preview each step with blur built in to avoid spoilers!
* Decide on the go whether a page is single or double
* Add a special page at the start and end of each chapter 
* Include page numbers and chapter names in the result
* Manage double panels so they will stay together
* Add anti-spoiler pages to help you when you cut or staple the printed pages
* Add RTL\LTR arrows and page numbers to make reading a breeze.
* Use many tools to solve common issues on manga files
* Export to PDF without any extra installs\tools!
* Run on Windows and Linux (with docker)

## Running on windows :
[Download the latest release](https://github.com/yonixw/MangaPrinter/releases/latest)

##  Running on Linux:
![](MangaPrinter.WpfGUI/Icons/More/linux.png) [Linux Readme](LinuxDockers/README.md)

## The process being explained with videos:

1. [Importing and fixing common issues (23:35)](https://youtu.be/vAnB7fNV588&list=PLTgFnSZ6Uv8Cd-5Lfo8xkQ0hmPBe5zBbD)
2. [Duplex Binding (17:40)](https://youtu.be/sBuj90tdme8&list=PLTgFnSZ6Uv8Cd-5Lfo8xkQ0hmPBe5zBbD)
3. [Booklet Binding (11:15)](https://youtu.be/UdmwzkMMWhg&list=PLTgFnSZ6Uv8Cd-5Lfo8xkQ0hmPBe5zBbD) - Assumes you saw Duplex
5. [Config Manager (10:17)](https://youtu.be/ACURi1TLLTU&list=PLTgFnSZ6Uv8Cd-5Lfo8xkQ0hmPBe5zBbD) - Optional
6. [Running from docker (Linux) (6:40)](https://youtu.be/nQXFGGVf52Y&list=PLTgFnSZ6Uv8Cd-5Lfo8xkQ0hmPBe5zBbD) - Optional

## The process explained with pictures: 

1. Import all image files and let the program auto detect doubles:

![](https://raw.githubusercontent.com/yonixw/MangaPrinter/master/ReadmeImages/v2_1.png)

2. Use a Tool - For Example, Choose the double-page aspect ratio cutoff with a histogram dialog 

![](https://raw.githubusercontent.com/yonixw/MangaPrinter/master/ReadmeImages/2.png)

3. Rebind all chapters with your flavor of extra pages (Intro, Outro, Anti spoiler)

![](https://raw.githubusercontent.com/yonixw/MangaPrinter/master/ReadmeImages/v2_3.png)

4. Export to pdf with one click! Result sample:

![](https://raw.githubusercontent.com/yonixw/MangaPrinter/master/ReadmeImages/4.png)

5. Print! 

## Performance
The software tries to use less than 1GB of Memory when possible. Here you can see an example CPU% and Memory usage during the export to pdf process:

![](https://raw.githubusercontent.com/yonixw/MangaPrinter/master/ReadmeImages/5Performance.png)
