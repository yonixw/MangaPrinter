
 import React from 'react';
// also exported from '@storybook/react' if you can deal with breaking changes in 6.1
import { Story, Meta } from '@storybook/react/types-6-0';

import { ChapterList } from './chapterlist';




import 'antd/dist/antd.css'; 
import { MangaChapter } from '../../lib/MangaChapter';
import { observable, IObservableArray, toJS } from 'mobx';

export default {
  title: 'Example/ChapterList',
  component: ChapterList,
  //argTypes: {
  //  backgroundColor: { control: 'color' },
  //},
} as Meta;

const Template: Story<{chapters:IObservableArray<MangaChapter>}> 
                    = (args) => 
                  <>
                    <ChapterList {...args} />
                    <button onClick={(e)=>console.log(toJS(args.chapters))}>
                      Log state
                    </button>
                  </>;

export const Example1 = Template.bind({});
const chapters1 = observable([] as MangaChapter[]);
chapters1.push(MangaChapter.mockChapter(1,"Chapter1",true,20,
  "C:\\Users\\Yoni\\Documents\\Mangas\\Mangas\\Mangas\\Mangas\\Karakai Jouzu no Takagi-san\\Ch.135 - Dropped Something"))
chapters1.push(MangaChapter.mockChapter(2,"Chapter2",false,70))
Example1.args = {
  chapters: chapters1
};
 