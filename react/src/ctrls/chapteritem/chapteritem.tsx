import React, { useState, useEffect } from 'react';
import rtlImage from '../../icons/RTL.png'
import ltrImage from '../../icons/LTR.png'
import styles from './chapteritem.module.css'
import { Button, Checkbox, List, Tooltip } from 'antd';
import {DeleteOutlined, EditOutlined} from '@ant-design/icons'

export interface ChapterItemProps {
    chapterID: number
    rtl: boolean
    name: string
    pageCount: number
    checked?: boolean
    onChecked? : (checked:boolean)=>void
    onDelete? : () => void
}


export const ChapterItem = (props:ChapterItemProps ) => 
{
  // Declare a new state variable, which we'll call "count"
  const [name, setName] = useState(props.name);
  const [rtl, setRTL] = useState(props.rtl);
  const [pageCount] = useState(props.pageCount);
  const [checked,setCheckedState] = useState(props.checked || false);

  const setChecked = (c:boolean) => 
  {
    setCheckedState(c);
    if (props.onChecked) props.onChecked(c);
  }

  const toggleRTL = ()=> {setRTL(!rtl)}

  const renameChapter = () => {
    const newName = prompt("Enter new name:",name);
    if (newName && newName != name)
      setName(newName);
  }

  /* componentDidMount\Update */ 
  useEffect(() => {
    
  });

  return (
    <List.Item>
      {/* <img src={rtlImage} alt={"RTL"}/>
      <span></span>
      <span></span> */}

      <div className={styles.flexh}>
          <div>
            <Checkbox 
              checked={checked} 
              onChange={(e)=>setChecked(e.target.checked)}>
            </Checkbox>
          </div>
          <div>
            <Tooltip placement="right" title="Reading direction: RTL\LTR">
              <img 
                onClick={toggleRTL}
                className={styles["reset-img"]}
                src={rtl ? rtlImage: ltrImage} alt={rtl ? "RTL":"LTR"}/>
            </Tooltip>
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
      <div className={styles["row-controls"]}>
        <Button onClick={renameChapter}>
          <EditOutlined /> Rename
        </Button>
        <Button danger onClick={(props.onDelete || function(){})}>
          <DeleteOutlined />
        </Button>
      </div>
    </List.Item>)

};


