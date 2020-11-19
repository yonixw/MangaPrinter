import React from 'react';
import singlePage from '../../icons/1Page.png'
import dblPage from '../../icons/2Page.png'
import styles from './pageitem.module.css'
import { Button, Checkbox, List, Tooltip } from 'antd';
import {DeleteOutlined} from '@ant-design/icons'
import { observer } from 'mobx-react';
import { MangaPage } from '../../lib/MangaPage';
import { OnChapter } from '../chapteritem/chapteritem';


export type PageItemArgs = {page: MangaPage, onRemove?: OnChapter}

export const PageItem = observer((props: PageItemArgs) => 
{
  return (
    <List.Item >
      {/* <img src={rtlImage} alt={"RTL"}/>
      <span></span>
      <span></span> */}
      <List.Item.Meta 
      description={
        <Tooltip title={props.page.ImagePath} trigger="click">
          {props.page.ImagePath.length<70?
          props.page.ImagePath:
          "..."+props.page.ImagePath.substr(-70)}
        </Tooltip>
      }  
      title={
        <div className={styles.flexh}>
            <div>
              <Checkbox 
                checked={props.page.checked} 
                onChange={(e)=>props.page.setCheck(e.target.checked)}>
              </Checkbox>
            </div>
            <div>
              <Tooltip placement="right" title="Single\Double Page">
                 <img 
                  onClick={props.page.toggleDouble}
                  className={styles["reset-img"]}
                  src={props.page.IsDouble ? dblPage: singlePage} 
                  alt={(props.page.IsDouble ? "Double":"Single")+" Page"}/> 
              </Tooltip>
            </div>
            <div>
            &nbsp;
            {props.page.Name} 

              </div>
        </div>
      }></List.Item.Meta>
      <div className={styles["row-controls"]}>
        <Button danger 
            onClick={()=>
              (props.onRemove || function(){})((props.page.id))}
              >
          <DeleteOutlined />
        </Button>
      </div>
    </List.Item>)

});


