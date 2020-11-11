import React from 'react';
// also exported from '@storybook/react' if you can deal with breaking changes in 6.1
import { Story, Meta } from '@storybook/react/types-6-0';

import { ChapterItem, ChapterItemProps } from './ctrl';
import { MangaChapter, MangaPage } from 'mangaprinterjs-lib/MangaObjects';

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
  chapter: MangaChapter.mockChapter(
    "Chapter1",
    true,
    25)
};

export const LTR = Template.bind({});
LTR.args = {
chapter: MangaChapter.mockChapter(
  "Chapter2",
  false,
  20)
};

export const PageWarn1 = Template.bind({});
PageWarn1.args = {
chapter: MangaChapter.mockChapter(
  "Chapter2",
  true,
  25)
};

export const PageWarn2 = Template.bind({});
PageWarn2.args = {
chapter: MangaChapter.mockChapter(
  "Chapter2",
  false,
  70)
};