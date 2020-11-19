import { makeObservable, observable, action, computed } from "mobx"
import { MangaPage } from "./MangaPage"



export class MangaChapter {
    id:number = -1
    name: string = "<New Chapter>"
    rtl: boolean = true
    pages: Array<MangaPage> = []
    folderPath: string = "<Added manually>"
    
    checked: boolean = false;

    constructor(id:number, name:string, rtl:boolean, path?: string) {
        this.id = id
        makeObservable(this, {
            name: observable,
            rtl: observable,
            pages: observable,
            checked: observable,
            pageCount: computed,

            rename:  action.bound,
            setCheck:  action.bound,
            toggleRTL: action.bound,
        })
        this.name = name;
        this.rtl = rtl
        if (path)
            this.folderPath=path;
    }

    rename(newName:string) {
        this.name = newName
    }

    setCheck(state:boolean) {
        this.checked = state
    }

    toggleRTL() { 
        this.rtl = !this.rtl
    }

    get pageCount() {
        return this.pages.length;
    }

    static mockChapter(id: number, name:string, rtl:boolean,
        pageCount: number, path?:string) 
        : MangaChapter {
        const result = new MangaChapter(id,name,rtl,path);
        for (let i = 0; i < pageCount; i++) {
            result.pages.push(new MangaPage());
        }
        return result;
    }
}