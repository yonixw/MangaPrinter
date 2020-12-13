import React from 'react';
import rtlImage from '../../icons/RTL.png'
import ltrImage from '../../icons/LTR.png'
import styles from './chapteritem.module.css'
import { Button, Checkbox, List, Tooltip } from 'antd';
import { DeleteOutlined, EditOutlined } from '@ant-design/icons'
import { MangaChapter } from '../../lib/MangaChapter';
import { observer } from 'mobx-react';
import { PromptDialog } from '../promptdialog/promptdialog';


export type OnChapter = (chpaterID?: number, value?: any) => void

export const ChapterItem = observer(
  ({ chapter, onRemove }
    : { chapter: MangaChapter, onRemove?: OnChapter }) => {

    const renameChptFlow = {
      renameChapter: (ok: boolean, newName: string) => {
        if (!ok || !newName) return;
        chapter.rename(newName)
      },
      renameChapterUI: (showDialog: () => void) => {
        return <Tooltip placement="bottom" title="Rename Chapter">
          <Button onClick={showDialog}>
            <EditOutlined /> Rename
                </Button>
        </Tooltip>
      }
    }

    const highlight = (e: React.MouseEvent<HTMLElement, MouseEvent>) => {
      if (e.currentTarget.tagName.toLowerCase() == "button" ||
        e.nativeEvent.composedPath()[0] === e.currentTarget) {
        console.log("Highlighting!")
        chapter.setSelected(true)
      }
    }

    return (
      <List.Item className={chapter.selected ? styles.highlighted : ""}>
        {/* <img src={rtlImage} alt={"RTL"}/>
      <span></span>
      <span></span> */}
        <List.Item.Meta
          description={
            <Tooltip title={chapter.folderPath} trigger="click">
              <span onClick={highlight}>
              {chapter.folderPath.length < 60 ?
                chapter.folderPath :
                "..." + chapter.folderPath.substr(-60)}
              </span>
            </Tooltip>
          }
          title={
            <div className={styles.flexh} onClick={highlight}>
              <div>
                <Checkbox
                  checked={chapter.checked}
                  onChange={(e) => chapter.setCheck(e.target.checked)}>
                </Checkbox>
              </div>
              <div>
                <Tooltip placement="right" title="RightToLeft (RTL)\LeftToRight (LTR)">
                  <img
                    onClick={chapter.toggleRTL}
                    className={styles["reset-img"]}
                    src={chapter.rtl ? rtlImage : ltrImage}
                    alt={chapter.rtl ? "RTL" : "LTR"} />
                </Tooltip>
              </div>
              <div>
                <button style={{display: "contents"}} onClick={highlight}>
                  &nbsp;
                  {chapter.name}

                  &nbsp;
                  {
                    (chapter.pageCount < 25) ?
                      (<span>[{chapter.pageCount} pages]</span>) :
                      (
                        (chapter.pageCount < 65) ?
                          (<b>[{chapter.pageCount} 👀 pages]</b>) :
                          (<b style={{ color: "red" }}>[{chapter.pageCount} 🛑 pages]</b>)
                      )
                  }
                </button>
              </div>
            </div>
          }></List.Item.Meta>
        <div className={styles["row-controls"]}>
          <PromptDialog
            title="Rename" desc="Change chapter name:"
            defaultValue={chapter.name} keepLast
            openUI={renameChptFlow.renameChapterUI}
            onUpdate={renameChptFlow.renameChapter}
          />
          <Button danger
            onClick={() =>
              (onRemove || function () { })((chapter.id))}
          >
            <DeleteOutlined />
          </Button>
        </div>
      </List.Item>)

  });


