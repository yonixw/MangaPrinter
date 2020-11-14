import React, { useState, useEffect } from 'react';
import './chapterlist.module.css'
import styles from './chapterlist.module.css'

import { Affix, Button, List, Spin } from 'antd'

import { ChapterItem, ChapterItemProps } from '../chapteritem/chapteritem';
import { DeleteFilled, FolderOpenOutlined, PlusSquareOutlined } from '@ant-design/icons';
import { removeItemOnce } from '../../utils/arrays';


export interface ChapterListProps {
  chapters?: ChapterItemProps[]
  onChaptersUpdate: (chapters: ChapterItemProps[]) => void
}

export const ChapterList = (props: ChapterListProps) => {
  const [chapters, setChapters] = useState(props.chapters)
  const [loading, setLoading] = useState(false);
  /*const [container] = useState(null);*/

  const addChapter = () => {
    const chapterName = prompt("Enter new chapter name:")
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
  }

  /* componentDidMount\Update */
  useEffect(() => {

  });

  return (
    <>
      <Affix offsetTop={10} /*target={()=>container}*/>
        <div className={styles["menu-buttons"]}>
          <Button type="primary">
            <FolderOpenOutlined/> Import Folders</Button>
          <Button onClick={addChapter}>
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
            key={item.chapterID} onDelete={onDelete}
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
