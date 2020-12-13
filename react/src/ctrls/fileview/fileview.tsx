import React, { useState} from 'react';
import './fileview.module.css'
import styles from './fileview.module.css'

import { observer } from 'mobx-react';
import { IObservableArray } from 'mobx';
import { MangaChapter } from '../../lib/MangaChapter';
import { Col, Row } from 'antd';
import { ChapterList } from '../chapterlist/chapterlist';
import { PageList } from '../pagelist/pagelist';

export const FileView = observer(({chapters}:{chapters:IObservableArray<MangaChapter>})=>{
    let selectedChapter = chapters.filter(c=>c.selected);
    return (<>
        <Row style={{}}>
            <Col span={12} style={{padding:"7px"}}>
                <ChapterList chapters={chapters} />
            </Col>
            <Col span={12} style={{padding:"7px", maxHeight:"99vh", overflowY:"scroll"}}>
                {
                    (!selectedChapter||selectedChapter.length<1)
                        ?
                    (<></>):
                    (<PageList chapter={selectedChapter[0]} />)
                }
            </Col>
        </Row>
    </>)
})