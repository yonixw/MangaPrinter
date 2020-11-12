import React, { useState, useEffect } from 'react';
import './chapterlist.module.css'

import { Checkbox, List, Spin } from 'antd'

import { ChapterItem, ChapterItemProps } from '../chapteritem/chapteritem';


export interface ChapterListProps {
  chapters?: ChapterItemProps[]
  onChaptersUpdate: (chapters: ChapterItemProps[]) => void
}




export const ChapterList = (props: ChapterListProps) => {
  const [chapters, setChapters] = useState(props.chapters)
  const [loading, setLoading] = useState(false);

  /* componentDidMount\Update */
  useEffect(() => {

  });

  return (
    <ul>
      <List
        dataSource={chapters}
        renderItem={item => {
          const myIndex = (chapters||[]).findIndex(e=>(e.chapterID==item.chapterID));
          return (
            <ChapterItem {...item} key={item.chapterID}></ChapterItem>
        )}}
      >
        {loading && (
          <div className="demo-loading-container">
            <Spin />
          </div>
        )}
      </List>
    </ul>)

};
