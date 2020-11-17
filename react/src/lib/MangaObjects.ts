import { makeObservable, observable, action, computed } from "mobx"


export class MangaPage {
    isDouble: boolean = false;
}


export class MangaChapter {
    id:number = -1
    name: string = "<New Chapter>"
    rtl: boolean = true
    pages: Array<MangaPage> = []
    
    checked: boolean = false;

    constructor(id:number, name:string, rtl:boolean) {
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

    static mockChapter(id: number, name:string, rtl:boolean, pageCount: number) 
        : MangaChapter {
        const result = new MangaChapter(id,name,rtl);
        for (let i = 0; i < pageCount; i++) {
            result.pages.push(new MangaPage());
        }
        return result;
    }
}