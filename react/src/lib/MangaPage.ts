import {  makeAutoObservable } from 'mobx';
import { MangaChapter } from './MangaChapter';


export class MangaPage {
    id: number = -1;
    checked: boolean = false;

    Name: string = "Default.png";
    IsDouble: boolean = false;
    AspectRatio: number = 1;
    ImagePath : string = "";
    ChildIndexStart : number = -1;
    ChildIndexEnd : number = -1;

    GetParent: ()=>MangaChapter;

    constructor(parent : ()=>MangaChapter, path?: string,id?: number, Name?: string) {
        this.GetParent = parent;
        makeAutoObservable(this);
        this.ImagePath = path || "";
        this.id = id || -1;
        this.Name = Name || this.Name;
    }

    rename = (newName:string) => {
        this.Name = newName
    }

    setCheck = (state:boolean) => {
        this.checked = state
    }

    setDouble = (isDouble: boolean) => {
        this.IsDouble = isDouble;
        this.GetParent().recalculateChildPagesIndexes();
    }
    
    toggleDouble =  () => {
        this.setDouble(!this.IsDouble);
    }

}

