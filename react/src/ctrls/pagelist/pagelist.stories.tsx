
 import React from 'react';
// also exported from '@storybook/react' if you can deal with breaking changes in 6.1
import { Story, Meta } from '@storybook/react/types-6-0';

import { PageList } from './pagelist';




import 'antd/dist/antd.css'; 
import { MangaChapter } from '../../lib/MangaChapter';
import { observable, IObservableArray, toJS } from 'mobx';

export default {
  title: 'Example/PageListList',
  component: PageList,
  //argTypes: {
  //  backgroundColor: { control: 'color' },
  //},
} as Meta;

const Template: Story<{chapter:MangaChapter}> 
                    = (args) => 
                  <>
                    <PageList {...args} />
                    <button onClick={(e)=>console.log(toJS(args.chapter.pages))}>
                      Log state
                    </button>
                  </>;

export const Example1 = Template.bind({});
const chapter1 = MangaChapter.mockChapter(1,"Chapter1",true,20,
  "C:\\Users\\Yoni\\Documents\\Mangas\\Mangas\\Mangas\\Mangas\\Karakai Jouzu no Takagi-san\\Ch.135 - Dropped Something");

Example1.args = {
  chapter: chapter1
};
 