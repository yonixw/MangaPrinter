import React, { useState} from 'react';
import './chapterlist.module.css'
import styles from './chapterlist.module.css'

import { Affix, Button, Checkbox, Divider, List, Spin, Tooltip } from 'antd';

import { ChapterItem, } from '../chapteritem/chapteritem';
import { DeleteFilled, FolderOpenOutlined, PlusSquareOutlined } from '@ant-design/icons';
import { removeItemOnce, removeItemAll } from '../../utils/arrays';
import { MangaChapter } from '../../lib/MangaObjects';
import { observer } from 'mobx-react';
import { IObservableArray, runInAction } from 'mobx';

import rtlImage from '../../icons/RTL.png'
import ltrImage from '../../icons/LTR.png'

export const grayScaleBlur = "grayscale(1) blur(2px)"

export const ChapterList = observer( 
  ({chapters}:{chapters:IObservableArray<MangaChapter>}) => {

  const [loading,setLoading] = useState(false)
  /*const [container] = useState(null);*/

  const newChapter = ():MangaChapter=> {
    const chapterName = prompt("Enter new chapter name:") || "Empty1"

    return new MangaChapter((new Date()).getTime(), chapterName,true)
  }

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

  const changeSelectedRTL = (rtl:boolean) => {
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

  return (
    <>
      <Affix offsetTop={10} /*target={()=>container}*/>
        <div className={styles["menu-buttons"]}>
          <Checkbox 
            checked={allSelected} indeterminate={someSelected}
            onChange={(e)=>setCheckAll(e.target.checked)}
            ></Checkbox>
          <Tooltip placement="bottomLeft" title="Delete Selected">
            <Button danger disabled={noneSelected} onClick={deleteSelected}>
              <DeleteFilled/>
            </Button>
          </Tooltip>
          <Tooltip placement="bottom" title="RTL Selected">
            <img 
                style={noneSelected? {filter: grayScaleBlur}:{}}
                onClick={()=>changeSelectedRTL(true)}
                className={styles["reset-img"]}
                src={rtlImage} 
                alt={"RTL"}/>
          </Tooltip>
          <Tooltip placement="bottom" title="LTR Selected">
            <img 
                style={noneSelected? {filter: grayScaleBlur}:{}}
                onClick={()=>changeSelectedRTL(false)}
                className={styles["reset-img"]}
                src={ltrImage} 
                alt={"LTR"}/>
          </Tooltip>
          <Tooltip placement="bottom" title="Add empty chapter">
            <Button onClick={()=>runInAction(()=>chapters.push(newChapter()))}>
              <PlusSquareOutlined/>Empty</Button>
          </Tooltip>
          <Button type="primary">
            <FolderOpenOutlined/>Add Folder(s)</Button>
        </div>
      </Affix>
      <Divider />
      <List
        dataSource={chapters.slice()}
        renderItem={item => 
          (
          <ChapterItem chapter={item}
            key={item.id} 
            onRemove={()=>runInAction(()=>removeItemOnce(chapters,(e)=>e.id===item.id))}
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
