import React, { useState} from 'react';
import './chapterlist.module.css'
import styles from './chapterlist.module.css'

import { Affix, Button, Checkbox, Divider, List, Popconfirm, Spin, Tooltip } from 'antd';

import { ChapterItem, } from '../chapteritem/chapteritem';
import { DeleteFilled, FolderOpenOutlined, PlusSquareOutlined } from '@ant-design/icons';
import { removeItemOnce, removeItemAll } from '../../utils/arrays';
import { MangaChapter } from '../../lib/MangaChapter';
import { observer } from 'mobx-react';
import { IObservableArray, runInAction } from 'mobx';

import rtlImage from '../../icons/RTL.png'
import ltrImage from '../../icons/LTR.png'
import { PromptDialog } from '../promptdialog/promptdialog';

export const grayScaleBlur = "grayscale(1) blur(2px)"

export const ChapterList = observer( 
  ({chapters}:{chapters:IObservableArray<MangaChapter>}) => {

  const [loading,setLoading] = useState(false)
  /*const [container] = useState(null);*/


  const checkedCount = chapters.slice().filter(e=>e.checked).length
  const chaptersCount = chapters.slice().length;

  const noneSelected = checkedCount ===0;
  const allSelected = checkedCount === chaptersCount;
  const someSelected = !allSelected && !noneSelected

  const setCheckAll = (c:boolean) => {
    chapters.slice().forEach(ch=>{
      ch.setCheck(c);
    })
  }

  const changeCheckedRTL = (rtl:boolean) => {
    runInAction(()=>{
      chapters.filter(e=>e.checked).forEach((e)=>{
        e.rtl = rtl;
      })
    })
  }

  const deleteSelected = () => {
    runInAction(()=>{
      removeItemAll(chapters, (e)=>e.checked)
    })
  }

  const onChapterSelect = (id:string|undefined, value?:any) => {
    chapters.filter(e=>e.id !== id).forEach((e)=>{
      e.setSelected(false);
    })
  }

  const newChpFlow = {
    addChapter : (ok:boolean,chapterName:string)=> {
      if(!ok || !chapterName) return;
      runInAction(()=> {
        chapters.push(
          new MangaChapter(
            (new Date()).getTime().toString(), chapterName,true
          )
        )
      })
    },
    emptyChapterUI:(showDialog:()=>void) => {
      return <Tooltip placement="bottom" title="Add empty chapter">
                <Button onClick={showDialog}>
                  <PlusSquareOutlined/>Empty</Button>
            </Tooltip>
    }
  }

  return (
    <>
      <Affix offsetTop={10} /*target={()=>container}*/>
        <div className={styles["menu-buttons"]}>
          <Checkbox 
            checked={allSelected} indeterminate={someSelected}
            onChange={(e)=>setCheckAll(e.target.checked)}
            ></Checkbox>
            <Popconfirm placement="bottomLeft" title="Delete Selected?" 
              onConfirm={deleteSelected}>
              <Button danger disabled={noneSelected} >
                <DeleteFilled/>
              </Button>
            </Popconfirm>
          <Tooltip placement="bottom" title="RTL Selected">
            <img 
                style={noneSelected? {filter: grayScaleBlur}:{}}
                onClick={()=>changeCheckedRTL(true)}
                className={styles["reset-img"]}
                src={rtlImage} 
                alt={"RTL"}/>
          </Tooltip>
          <Tooltip placement="bottom" title="LTR Selected">
            <img 
                style={noneSelected? {filter: grayScaleBlur}:{}}
                onClick={()=>changeCheckedRTL(false)}
                className={styles["reset-img"]}
                src={ltrImage} 
                alt={"LTR"}/>
          </Tooltip>
          <PromptDialog 
              title="New chapter"
              desc="Enter new chapter name:" defaultValue="Empty1"
              openUI={newChpFlow.emptyChapterUI} 
              onUpdate={newChpFlow.addChapter}
          />
          
        </div>
      </Affix>
      <Divider />
      <List
        dataSource={chapters.slice()}
        renderItem={item => 
          (
          <ChapterItem chapter={item}
            key={item.id} 
            onSelect={onChapterSelect}
            onRemove={
              ()=>runInAction(
                ()=>removeItemOnce(chapters,(e)=>e.id===item.id))}
          />)
        }>

        {loading && (
          <div className="demo-loading-container">
            <Spin />
          </div>
        )}

      </List>
    </>)

});
