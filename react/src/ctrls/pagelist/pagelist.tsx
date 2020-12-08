import React, { useState} from 'react';
import './pagelist.module.css'
import styles from './pagelist.module.css'

import { Affix, Button, Checkbox, Divider, List, Popconfirm, Spin, Tooltip } from 'antd';

import { ChapterItem, } from '../chapteritem/chapteritem';
import { DeleteFilled, FolderOpenOutlined, PlusSquareOutlined } from '@ant-design/icons';
import { removeItemOnce, removeItemAll } from '../../utils/arrays';
import { MangaChapter } from '../../lib/MangaChapter';
import { observer } from 'mobx-react';
import { IObservableArray, runInAction } from 'mobx';

import singlePage from '../../icons/1Page.png'
import dblPage from '../../icons/2Page.png'
import { PromptDialog } from '../promptdialog/promptdialog';
import { MangaPage } from '../../lib/MangaPage';
import { Double } from '../pageitem/pageitem.stories';
import { PageItem } from '../pageitem/pageitem';

export const grayScaleBlur = "grayscale(1) blur(2px)"

export const PageList = observer( 
  ({chapter}:{chapter:MangaChapter}) => {

  const [loading,setLoading] = useState(false)
  /*const [container] = useState(null);*/


  const checkedCount = chapter.pages.slice().filter(e=>e.checked).length
  const chaptersCount = chapter.pages.slice().length;

  const noneSelected = checkedCount ===0;
  const allSelected = checkedCount === chaptersCount;
  const someSelected = !allSelected && !noneSelected

  const setCheckAll = (c:boolean) => {
    chapter.pages.slice().forEach(ch=>{
      ch.setCheck(c);
    })
  }

  const changeSelectedDouble = (isDouble:boolean) => {
    runInAction(()=>{
      chapter.pages.filter(e=>e.checked).forEach((e)=>{
        e.setDouble(isDouble)
      })
    })
  }

  const deleteSelected = () => {
    runInAction(()=>{
      removeItemAll(chapter.pages, (e)=>e.checked)
      chapter.recalculateChildPagesIndexes();
    })
  }

  const newPageFlow = {
    addPage : (ok:boolean,pagePath:string)=> {
      if(!ok || !pagePath) return;
      runInAction(()=> {
        chapter.pages.push(
          new MangaPage(
            ()=>chapter, pagePath,(new Date()).getTime()
          )
        )
      })
    },
    emptyChapterUI:(showDialog:()=>void) => {
      return <Tooltip placement="bottom" title="Add empty page">
                <Button onClick={showDialog}>
                  <PlusSquareOutlined/>Empty</Button>
            </Tooltip>
    }
  }

  return (
    <>
      <Affix offsetTop={10} /*target={()=>container}*/>
        <div className={styles["menu-buttons"]}>
          <Checkbox 
            checked={allSelected} indeterminate={someSelected}
            onChange={(e)=>setCheckAll(e.target.checked)}
            ></Checkbox>
            <Popconfirm placement="bottomLeft" title="Delete Selected?" 
              onConfirm={deleteSelected}>
              <Button danger disabled={noneSelected} >
                <DeleteFilled/>
              </Button>
            </Popconfirm>
          <Tooltip placement="bottom" title="Single Page Selected">
            <img 
                style={noneSelected? {filter: grayScaleBlur}:{}}
                onClick={()=>changeSelectedDouble(false)}
                className={styles["reset-img"]}
                src={singlePage} 
                alt={"Single Page"}/>
          </Tooltip>
          <Tooltip placement="bottom" title="Double Page Selected">
            <img 
                style={noneSelected? {filter: grayScaleBlur}:{}}
                onClick={()=>changeSelectedDouble(true)}
                className={styles["reset-img"]}
                src={dblPage} 
                alt={"Double Page"}/>
          </Tooltip>
          <PromptDialog 
              title="New page"
              desc="Enter new page path:" defaultValue="./page.png"
              openUI={newPageFlow.emptyChapterUI} 
              onUpdate={newPageFlow.addPage}
          />
          
        </div>
      </Affix>
      <Divider />
      <List
        dataSource={chapter.pages.slice()}
        renderItem={item => 
          (
          <PageItem page={item}
            key={item.id} 
            onRemove={
              ()=>runInAction(
                ()=>{
                  removeItemOnce(chapter.pages,(e)=>e.id===item.id);
                  chapter.recalculateChildPagesIndexes();
                })
                }
          />)
        }>

        {loading && (
          <div className="demo-loading-container">
            <Spin />
          </div>
        )}

      </List>
    </>)

});
