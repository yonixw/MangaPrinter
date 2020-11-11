import React from 'react';
// also exported from '@storybook/react' if you can deal with breaking changes in 6.1
import { Story, Meta } from '@storybook/react/types-6-0';

import { ChapterItem, ChapterItemProps } from './chapteritem';

export default {
  title: 'Example/ChapterItem',
  component: ChapterItem,
  //argTypes: {
  //  backgroundColor: { control: 'color' },
  //},
} as Meta;

const Template: Story<ChapterItemProps> = (args) => <ChapterItem {...args} />;

export const RTL = Template.bind({});
RTL.args = {
  name: "Chapter1",
  pageCount: 12,
  rtl: true
};

export const LTR = Template.bind({});
LTR.args = {
  name: "Chapter2",
  pageCount: 20,
  rtl: false
};

export const PageWarn1 = Template.bind({});
PageWarn1.args = {
  name: "Chapter2",
  pageCount: 25,
  rtl: true
};

export const PageWarn2 = Template.bind({});
PageWarn2.args = {
  name: "Chapter2",
  pageCount: 70,
  rtl: false
};

