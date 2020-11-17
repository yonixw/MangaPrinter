import React, { useState, useEffect, useReducer } from 'react';
import './chapterlist.module.css'
import styles from './chapterlist.module.css'

import { Affix, Button, List, Spin } from 'antd'

import { ChapterItem, } from '../chapteritem/chapteritem';
import { DeleteFilled, FolderOpenOutlined, PlusSquareOutlined } from '@ant-design/icons';
import { removeItemOnce, ArrayReducer, useReduceArr, RActions } from '../../utils/arrays';
import { MangaChapter } from '../../lib/MangaObjects';
import { observer } from 'mobx-react';
import { IObservableArray, runInAction, toJS } from 'mobx';



export const ChapterList = observer( 
  ({chapters}:{chapters:IObservableArray<MangaChapter>}) => {

  const [loading,setLoading] = useState(false)
  /*const [container] = useState(null);*/

  const newChapter = ():MangaChapter=> {
    const chapterName = prompt("Enter new chapter name:") || "Empty1"

    console.log(toJS(chapters)) // use slice() to print

    return new MangaChapter((new Date()).getTime(), chapterName,true)
  }



  return (
    <>
      <Affix offsetTop={10} /*target={()=>container}*/>
        <div className={styles["menu-buttons"]}>
          <Button type="primary">
            <FolderOpenOutlined/> Import Folders</Button>
          <Button onClick={()=>runInAction(()=>chapters.push(newChapter()))}>
            <PlusSquareOutlined/> Add Empty</Button>
          <Button danger>
            <DeleteFilled/> Clear all</Button>
        </div>
      </Affix>
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
