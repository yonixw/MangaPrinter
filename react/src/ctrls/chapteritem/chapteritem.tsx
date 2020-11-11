import React, { useState, useEffect } from 'react';
import rtlImage from '../../icons/RTL.png'
import ltrImage from '../../icons/LTR.png'
import styles from './ctrl.module.css'

export interface ChapterItemProps {
    chapterID: number
    rtl?: boolean
    name: string
    pageCount: number
}


export const ChapterItem = (props:ChapterItemProps ) => 
{
  // Declare a new state variable, which we'll call "count"
  const rtl = props.rtl;
  const name = props.name;
  const pageCount = props.pageCount;

  console.log(props);

  /* componentDidMount\Update */ 
  useEffect(() => {
    
  });

  return (
    <div>
      {/* <img src={rtlImage} alt={"RTL"}/>
      <span></span>
      <span></span> */}

      <div className={styles.flexh}>
          <div>
            <img 
              className={styles["reset-img"]}
              src={rtl ? rtlImage: ltrImage} 
              alt={rtl ? "RTL":"LTR"}/>
          </div>
          <div>
          &nbsp;
          {name} 
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
