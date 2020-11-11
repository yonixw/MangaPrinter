import React, { useState, useEffect } from 'react';
import styles from './ctrl.module.css'
import { ChapterItem, ChapterItemProps } from '../chapteritem/chapteritem';


export interface ChapterListProps {
    chapters? : ChapterItemProps[]
    onChaptersUpdate: (chapters:ChapterItemProps[]) => void
}


export const ChapterList = (props:ChapterListProps ) => 
{
  const [chapters, setChapters] = useState(props.chapters)

  /* componentDidMount\Update */ 
  useEffect(() => {
    
  });

  return (
    <ul>
      {chapters?.map((ch)=>{
        return (<ChapterItem {...ch} key={ch.chapterID}></ChapterItem>)
      })}
    </ul>)

};
