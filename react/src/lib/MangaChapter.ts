import { makeAutoObservable } from 'mobx';
import { MangaPage } from "./MangaPage"



export class MangaChapter {
    id:number = -1
    name: string = "<New Chapter>"
    rtl: boolean = true
    pages: Array<MangaPage> = []
    folderPath: string = "<Added manually>"
    
    checked: boolean = false;
    selected: boolean = false;

    constructor(id:number, name:string, rtl:boolean, path?: string) {
        this.id = id
        this.name = name;
        this.rtl = rtl
        if (path)
            this.folderPath=path;
        makeAutoObservable(this);
    }

    setSelected = (selected:boolean) => {
        this.selected = selected;
    }

    rename = (newName:string) => {
        this.name = newName
    }

    setCheck = (state:boolean) => {
        this.checked = state
    }

    toggleRTL= () => { 
        this.rtl = !this.rtl
    }

    recalculateChildPagesIndexes = () => {
        let index = 1;
        this.pages.forEach(p=> {
            p.ChildIndexStart = index;
            if (p.IsDouble) {
                index++;
            }
            p.ChildIndexEnd = index;
            index++;
        })
    }

    get pageCount() {
        //return this.pages.length;
        let result = 0;
        result += this.pages.filter(p=>p.IsDouble).length *2;
        result += this.pages.filter(p=>!p.IsDouble).length;
        return result;
    }

    addEmptyPage = () => {
        let that=this;
        this.pages.push(new MangaPage(()=>that))
    }

    static mockChapter(id: number, name:string, rtl:boolean,
        pageCount: number, path?:string) 
        : MangaChapter {
        const result = new MangaChapter(id,name,rtl,path);
        for (let i = 0; i < pageCount; i++) {
            result.pages.push(
                new MangaPage(()=>result,"Some Path"+i,i,`Default ${i}.png`)
            );
        }
        return result;
    }
}