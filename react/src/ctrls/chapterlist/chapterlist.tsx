import React, { useState, useEffect, useReducer } from 'react';
import './chapterlist.module.css'
import styles from './chapterlist.module.css'

import { Affix, Button, Checkbox, Divider, List, Spin, Tooltip } from 'antd';

import { ChapterItem, } from '../chapteritem/chapteritem';
import { DeleteFilled, FolderOpenOutlined, PlusSquareOutlined } from '@ant-design/icons';
import { removeItemOnce, ArrayReducer, useReduceArr, RActions } from '../../utils/arrays';
import { MangaChapter } from '../../lib/MangaObjects';
import { observer } from 'mobx-react';
import { IObservableArray, runInAction, toJS } from 'mobx';

import rtlImage from '../../icons/RTL.png'
import ltrImage from '../../icons/LTR.png'



export const ChapterList = observer( 
  ({chapters}:{chapters:IObservableArray<MangaChapter>}) => {

  const [loading,setLoading] = useState(false)
  /*const [container] = useState(null);*/

  const newChapter = ():MangaChapter=> {
    const chapterName = prompt("Enter new chapter name:") || "Empty1"

    return new MangaChapter((new Date()).getTime(), chapterName,true)
  }

  const noneSelected = chapters.slice().filter(e=>e.checked).length==0;

  return (
    <>
      <Affix offsetTop={10} /*target={()=>container}*/>
        <div className={styles["menu-buttons"]}>
          <Checkbox></Checkbox>
          <Tooltip placement="bottomLeft" title="Delete Selected">
            <Button danger disabled={noneSelected}>
              <DeleteFilled/>
            </Button>
          </Tooltip>
          <Tooltip placement="bottom" title="RTL Selected">
            <img 
                //onClick={chapter.toggleRTL}
                className={styles["reset-img"]}
                src={rtlImage} 
                alt={"RTL"}/>
          </Tooltip>
          <Tooltip placement="bottom" title="LTR Selected">
            <img 
//                onClick={chapter.toggleRTL}
                className={styles["reset-img"]}
                src={ltrImage} 
                alt={"LTR"}/>
          </Tooltip>
          <Tooltip placement="bottom" title="Add empty chapter">
            <Button onClick={()=>runInAction(()=>chapters.push(newChapter()))}>
              <PlusSquareOutlined/>Empty</Button>
          </Tooltip>
          <Button type="primary">
            <FolderOpenOutlined/>Add Folders</Button>
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
