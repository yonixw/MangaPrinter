export class MangaPage {
    isDouble: boolean = false;
}


export class MangaChapter {
    name: string = "<New Chapter>"
    rtl: boolean = true
    pages: Array<MangaPage> = []

    static mockChapter(name:string, rtl:boolean, pageCount: number) 
        : MangaChapter {
        const result = new MangaChapter();
        result.name = name;
        result.rtl = rtl;
        result.pages = [];
        for (let i = 0; i < pageCount; i++) {
            result.pages.push(new MangaPage());
        }
        return result;
    }
}