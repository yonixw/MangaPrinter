import React from 'react';
// also exported from '@storybook/react' if you can deal with breaking changes in 6.1
import { Story, Meta } from '@storybook/react/types-6-0';

import { ChapterList, ChapterListProps } from './chapterlist';

export default {
  title: 'Example/ChapterList',
  component: ChapterList,
  //argTypes: {
  //  backgroundColor: { control: 'color' },
  //},
} as Meta;

const Template: Story<ChapterListProps> = (args) => <ChapterList {...args} />;

export const Example1 = Template.bind({});
Example1.args = {
  chapters: [
    {chapterID:1,name:"Chapter1",pageCount:20,rtl: true},
    {chapterID:2,name:"Chapter1",pageCount:20,rtl: true}
  ]
};
