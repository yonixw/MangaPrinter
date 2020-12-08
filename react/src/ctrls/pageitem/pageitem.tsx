import React from 'react';
import singlePage from '../../icons/1Page.png'
import dblPage from '../../icons/2Page.png'
import styles from './pageitem.module.css'
import { Button, Checkbox, List, Tooltip } from 'antd';
import {DashboardTwoTone, DeleteOutlined, FieldNumberOutlined} from '@ant-design/icons'
import { observer } from 'mobx-react';
import { MangaPage } from '../../lib/MangaPage';
import { OnChapter } from '../chapteritem/chapteritem';
import { stringify } from 'querystring';
import { round } from '../../utils/numbers';


export type PageItemArgs = {page: MangaPage, onRemove?: OnChapter}

export const PageItem = observer((props: PageItemArgs) => 
{
  return (
    <List.Item >
      {/* <img src={rtlImage} alt={"RTL"}/>
      <span></span>
      <span></span> */}
      <List.Item.Meta 

      title={
        <div className={styles.flexh}>
            <div>
              <Checkbox 
                checked={props.page.checked} 
                onChange={(e)=>props.page.setCheck(e.target.checked)}>
              </Checkbox>
            </div>
            <div>
              <Tooltip placement="bottomLeft" title="Single\Double Page">
                 <img 
                  onClick={props.page.toggleDouble}
                  className={styles["reset-img"]}
                  src={props.page.IsDouble ? dblPage: singlePage} 
                  alt={(props.page.IsDouble ? "Double":"Single")+" Page"}/> 
              </Tooltip>
            </div>
            <div>
            <>({
              props.page.IsDouble ?
              (<i>
                <FieldNumberOutlined />{props.page.ChildIndexStart}-
                <FieldNumberOutlined />{props.page.ChildIndexEnd}
              </i>)
              :
              (<><FieldNumberOutlined />{props.page.ChildIndexStart}</>)
            })
            </>

            &nbsp;
            <Tooltip title={props.page.ImagePath} trigger="click">
              {props.page.ImagePath.length<35?
              props.page.ImagePath:
              "..."+props.page.ImagePath.substr(-35)}
            </Tooltip>
            &nbsp; 
            
            </div>
        </div>
      }></List.Item.Meta>
      <div className={styles["row-controls"]}>
	     <Tooltip title="Aspect-ratio (Width/Height)" placement="bottom">
            <DashboardTwoTone />{round(props.page.AspectRatio)}
            </Tooltip>
        <Button danger 
            onClick={()=>
              (props.onRemove || function(){})((props.page.id))}
              >
          <DeleteOutlined />
        </Button>
      </div>
    </List.Item>)

});


