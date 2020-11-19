import {  makeAutoObservable } from 'mobx';


export class MangaPage {
    id: number = -1;
    checked: boolean = false;

    Name: string = "Default.png";
    IsDouble: boolean = false;
    AspectRatio: number = 1;
    ImagePath : string = "";
    ChildIndexStart : number = -1;
    ChildIndexEnd : number = -1;


    constructor() {
        makeAutoObservable(this);
    }

    rename = (newName:string) => {
        this.Name = newName
    }

    setCheck = (state:boolean) => {
        this.checked = state
    }

    setDouble = (isDouble: boolean) => {
        this.IsDouble = isDouble;
    }

    
    toggleDouble =  () => {
        this.setDouble(!this.IsDouble);
    }

}

