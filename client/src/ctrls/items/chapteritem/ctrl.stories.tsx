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
    25)
};
