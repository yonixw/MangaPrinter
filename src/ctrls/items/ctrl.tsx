import React, { useState, useEffect } from 'react';
import rtlImage from '../../icons/RTL.png'
import ltrImage from '../../icons/LTR.png'
import styles from './ctrl.module.css'

export interface ChapterItemProps {
  rtl?: boolean
  name: string
  pageCount: number
}


export const ChapterItem = (
  props:ChapterItemProps = {
    rtl: true, name: "Empty chapter", pageCount:0
  }) => 
{
  // Declare a new state variable, which we'll call "count"
  const [count, setCount] = useState(0);

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
              src={props.rtl ? rtlImage: ltrImage} 
              alt={"RTL"}/>
          </div>
          <div>
          &nbsp;
          {props.name} 
          &nbsp;
          {
            (props.pageCount < 25) ? 
            (<span>[{props.pageCount} pages]</span>) :
            (
              (props.pageCount < 65) ? 
              (<b>[{props.pageCount} 👀 pages]</b>) :
              (<b style={{color:"red"}}>[{props.pageCount} 🛑 pages]</b>)
              )
            }
            </div>
      </div>
    </div>)

};
