import React, { useState, useEffect } from 'react';
import rtlImage from '../../icons/RTL.png'
import ltrImage from '../../icons/LTR.png'
import styles from './chapter.item.module.css'

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
          <div><img src={props.rtl ? rtlImage: ltrImage} alt={"RTL"}/></div>
          <div>{props.name}</div>
          <div>
          {
            (props.pageCount < 25) ? 
              (<span>[{props.pageCount}] </span>) :
              (
              (props.pageCount < 65) ? 
                (<b>[{props.pageCount} 👀] </b>) :
                (<b style={{color:"red"}}>[{props.pageCount} 🛑] </b>)
              )
          }
          </div>
      </div>
    </div>)

};
