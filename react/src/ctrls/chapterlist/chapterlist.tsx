import React, { useState, useEffect, useReducer } from 'react';
import './chapterlist.module.css'
import styles from './chapterlist.module.css'

import { Affix, Button, List, Spin } from 'antd'

import { ChapterItem, ChapterItemProps } from '../chapteritem/chapteritem';
import { DeleteFilled, FolderOpenOutlined, PlusSquareOutlined } from '@ant-design/icons';
import { removeItemOnce, ArrayReducer, useReduceArr, RActions } from '../../utils/arrays';


export interface ChapterListProps {
  chapters?: ChapterItemProps[]
  onChaptersUpdate: (chapters: ChapterItemProps[]) => void
}

export const ChapterList = (props: ChapterListProps) => {
  //const [chapters, setChapters] = useState(props.chapters)

  const initialChapters = (props.chapters||new Array<ChapterItemProps>());
  const [chapters,setChpater] = useReduceArr(
      initialChapters
  );
  const [loading, setLoading] = useState(false);
  /*const [container] = useState(null);*/

  const newChapter = ():ChapterItemProps=> {
    const chapterName = prompt("Enter new chapter name:") || "Empty1"
    return {
      chapterID: (new Date()).getTime(), // TODO: Replace with guid
      name: chapterName,
      rtl: false,
      pageCount: 0
    }
  }

  /* const addChapter = () => {
    if (!chapterName) return;
    
    const newChapter : ChapterItemProps = {
      chapterID: (new Date()).getTime(), // TODO: Replace with guid
      name: chapterName,
      rtl: false,
      pageCount: 0
    }
    setChapters([...(chapters||[]), newChapter]);
  }

  const onDelete =(id:number) => {
    if (chapters) {
      setChapters(
        [...removeItemOnce(chapters, (e)=>e.chapterID===id)]
      );
    }
  } */

  /* componentDidMount\Update */
  useEffect(() => {

  });

  return (
    <>
      <Affix offsetTop={10} /*target={()=>container}*/>
        <div className={styles["menu-buttons"]}>
          <Button type="primary">
            <FolderOpenOutlined/> Import Folders</Button>
          <Button onClick={
              ()=>setChpater({type:RActions.INSERT, payload: newChapter()})
          }>
            <PlusSquareOutlined/> Add Empty</Button>
          <Button danger>
            <DeleteFilled/> Clear all</Button>
        </div>
      </Affix>
      <List
        dataSource={chapters}
        renderItem={item => 
          (
          <ChapterItem {...item} 
            key={item.chapterID} onDelete={
              ()=>setChpater({type:RActions.REMOVE, payload: item})
            }
          />)
        }>

        {loading && (
          <div className="demo-loading-container">
            <Spin />
          </div>
        )}

      </List>
    </>)

};
