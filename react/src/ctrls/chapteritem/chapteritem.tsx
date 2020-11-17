import React from 'react';
import rtlImage from '../../icons/RTL.png'
import ltrImage from '../../icons/LTR.png'
import styles from './chapteritem.module.css'
import { Button, Checkbox, List, Tooltip } from 'antd';
import {DeleteOutlined, EditOutlined} from '@ant-design/icons'
import { MangaChapter } from '../../lib/MangaObjects';
import { observer } from 'mobx-react';


export type OnChapter = (chpaterID?:number, value?:any)=>void

export const ChapterItem = observer(
  ({chapter,onRemove}
      :{chapter:MangaChapter,onRemove?:OnChapter}) => 
{

  const renameChapter = () => {
    const newName = prompt("Enter new name:",chapter.name);
    if (newName && newName !== chapter.name)
      chapter.rename(newName)
  }

  /* componentDidMount\Update */ 
  /* useEffect(() => {
    
  });
 */
  return (
    <List.Item>
      {/* <img src={rtlImage} alt={"RTL"}/>
      <span></span>
      <span></span> */}

      <div className={styles.flexh}>
          <div>
            <Checkbox 
              checked={chapter.checked} 
              onChange={(e)=>chapter.setCheck(e.target.checked)}>
            </Checkbox>
          </div>
          <div>
            <Tooltip placement="right" title="Reading direction: RTL\LTR">
              <img 
                onClick={chapter.toggleRTL}
                className={styles["reset-img"]}
                src={chapter.rtl ? rtlImage: ltrImage} 
                alt={chapter.rtl ? "RTL":"LTR"}/>
            </Tooltip>
          </div>
          <div>
          &nbsp;
          {chapter.name} 

          &nbsp;
          {
            (chapter.pageCount < 25) ? 
            (<span>[{chapter.pageCount} pages]</span>) :
            (
              (chapter.pageCount < 65) ? 
              (<b>[{chapter.pageCount} 👀 pages]</b>) :
              (<b style={{color:"red"}}>[{chapter.pageCount} 🛑 pages]</b>)
              )
            }
            </div>
      </div>
      <div className={styles["row-controls"]}>
        <Button onClick={renameChapter}>
          <EditOutlined /> Rename
        </Button>
        <Button danger 
            onClick={()=>
              (onRemove || function(){})((chapter.id))}
              >
          <DeleteOutlined />
        </Button>
      </div>
    </List.Item>)

});


