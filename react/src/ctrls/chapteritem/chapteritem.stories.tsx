import React from 'react';
// also exported from '@storybook/react' if you can deal with breaking changes in 6.1
import { Story, Meta } from '@storybook/react/types-6-0';

import { ChapterItem } from './chapteritem';
import { MangaChapter } from '../../lib/MangaObjects';

import 'antd/dist/antd.css'; 

export default {
  title: 'Example/ChapterItem',
  component: ChapterItem,
  //argTypes: {
  //  backgroundColor: { control: 'color' },
  //},
} as Meta;

const Template: Story<{ chapter: MangaChapter; }> = (args) => <ChapterItem {...args} />;

export const RTL = Template.bind({});
RTL.args = {
  chapter: new MangaChapter(0,"Chapter 1",true)
};

export const LTR = Template.bind({});
LTR.args = {
  chapter: new MangaChapter(0,"Chapter 2",false)
};


export const PageWarn1 = Template.bind({});
PageWarn1.args = {
  chapter: MangaChapter.mockChapter(0,"Chapter 2",false,25)
};

export const PageWarn2 = Template.bind({});
PageWarn2.args = {
  chapter: MangaChapter.mockChapter(0,"Chapter 2",false,67)
};
 
