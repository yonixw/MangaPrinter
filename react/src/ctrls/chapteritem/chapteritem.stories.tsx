import React from 'react';
// also exported from '@storybook/react' if you can deal with breaking changes in 6.1
import { Story, Meta } from '@storybook/react/types-6-0';

import { ChapterItem } from './chapteritem';
import { MangaChapter } from '../../lib/MangaChapter';

import 'antd/dist/antd.css'; 
import { Button } from 'antd';

export default {
  title: 'Example/ChapterItem',
  component: ChapterItem,
  //argTypes: {
  //  backgroundColor: { control: 'color' },
  //},
} as Meta;


const TemplateDef: Story<{ chapter: MangaChapter; }> = (args) => 
<>
  <ChapterItem {...args} />
  <Button onClick={args.chapter.addEmptyPage}>Add Page</Button>
  <Button onClick={()=>args.chapter.setSelected(false)}>Unselect</Button>
</>

const TemplateSimple: Story<{ chapter: MangaChapter; }> = (args) => <ChapterItem {...args} />;

export const RTL = TemplateDef.bind({});
RTL.args = {
  chapter: MangaChapter.mockChapter(0,"Chapter 1",true,24,"../Some/Path/To/Chapter/")
};

export const LTR = TemplateSimple.bind({});
LTR.args = {
  chapter: new MangaChapter(0,"Chapter 2",false)
};


export const PageWarn1 = TemplateSimple.bind({});
PageWarn1.args = {
  chapter: MangaChapter.mockChapter(0,"Chapter 2",false,25)
};

export const PageWarn2 = TemplateSimple.bind({});
PageWarn2.args = {
  chapter: MangaChapter.mockChapter(0,"Chapter 2",false,67)
};
 
