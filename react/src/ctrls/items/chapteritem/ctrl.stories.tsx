import React from 'react';
// also exported from '@storybook/react' if you can deal with breaking changes in 6.1
import { Story, Meta } from '@storybook/react/types-6-0';

import { ChapterItem, ChapterItemProps } from './ctrl';
import { MangaChapter } from "../../../lib/MangaObjects"

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
    20)
};

export const LTR = Template.bind({});
LTR.args = {
  chapter: MangaChapter.mockChapter(
    "Chapter2",
    true,
    24)
};

export const PageCountWarn1 = Template.bind({});
PageCountWarn1.args = {
  chapter: MangaChapter.mockChapter(
    "Chapter3",
    false,
    25)
};

export const PageCountWarn2 = Template.bind({});
PageCountWarn2.args = {
  chapter: MangaChapter.mockChapter(
    "Chapter4",
    true,
    65)
};
