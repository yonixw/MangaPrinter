export declare class MangaPage {
    isDouble: boolean;
}
export declare class MangaChapter {
    name: string;
    rtl: boolean;
    pages: Array<MangaPage>;
    static mockChapter(name: string, rtl: boolean, pageCount: number): MangaChapter;
}
