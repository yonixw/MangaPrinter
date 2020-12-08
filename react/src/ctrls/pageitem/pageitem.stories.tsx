import React from 'react';
// also exported from '@storybook/react' if you can deal with breaking changes in 6.1
import { Story, Meta } from '@storybook/react/types-6-0';

import { PageItem, PageItemArgs } from './pageitem';
import { MangaPage } from '../../lib/MangaPage';
import { toJS } from 'mobx';
import { MangaChapter } from '../../lib/MangaChapter';


export default {
  title: 'Example/PageItem',
  component: PageItem,
  //argTypes: {
  //  backgroundColor: { control: 'color' },
  //},
} as Meta;

const Template: Story<PageItemArgs> = (args) =>
  <>
   <PageItem {...args} /> 
   <button onClick={(e)=>console.log(toJS(args.page))}>
                      Log state
                    </button>
  </>;

const Parents = [
  new MangaChapter(-1,"Chapter 1",false)
]

export const Single = Template.bind({});
const ex1 = new MangaPage(()=>Parents[0])
Parents[0].pages.push(ex1);
ex1.ImagePath = "C:/Manga/Manga/Manga//Manga/Manga/Manga/Manga/Manga/Manga/Manga/Path/01.png"
ex1.ChildIndexEnd = ex1.ChildIndexStart = 22
ex1.AspectRatio = 0.7785115
Single.args = {
  page: ex1
};

export const Double = Template.bind({});
const ex2 = new MangaPage(()=>Parents[0])
Parents[0].pages.push(ex2);
ex2.IsDouble = true;
ex2.ChildIndexStart = 5
ex2.ChildIndexEnd = 6
ex2.AspectRatio = 1.116661
Double.args = {
  page: ex2
};
