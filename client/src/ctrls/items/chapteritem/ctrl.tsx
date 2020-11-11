import React, { useState, useEffect } from 'react';
import rtlImage from '../../../icons/RTL.png'
import ltrImage from '../../../icons/LTR.png'
import styles from './ctrl.module.css'
import { MangaChapter} from "../../../lib/MangaObjects"

export interface ChapterItemProps {
  chapter: MangaChapter
}


export const ChapterItem = (
  props:ChapterItemProps = {
    chapter: { rtl: true, name: "Empty chapter", pages: []}
  }) => 
{
  // Declare a new state variable, which we'll call "count"
  const [count, setCount] = useState(0);

  /* componentDidMount\Update */ 
  useEffect(() => {
    
  });

  const pageCount = props.chapter.pages.length;
  return (
    <div>
      {/* <img src={rtlImage} alt={"RTL"}/>
      <span></span>
      <span></span> */}

      <div className={styles.flexh}>
          <div>
            <img 
              className={styles["reset-img"]}
              src={props.chapter.rtl ? rtlImage: ltrImage} 
              alt={"RTL"}/>
          </div>
          <div>
          &nbsp;
          {props.chapter.name} 
          &nbsp;
          {
            (pageCount < 25) ? 
            (<span>[{pageCount} pages]</span>) :
            (
              (pageCount < 65) ? 
              (<b>[{pageCount} 👀 pages]</b>) :
              (<b style={{color:"red"}}>[{pageCount} 🛑 pages]</b>)
              )
            }
            </div>
      </div>
    </div>)

};
