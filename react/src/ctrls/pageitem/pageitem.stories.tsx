import React from 'react';
// also exported from '@storybook/react' if you can deal with breaking changes in 6.1
import { Story, Meta } from '@storybook/react/types-6-0';

import { PageItem, PageItemArgs } from './pageitem';
import { MangaPage } from '../../lib/MangaPage';
import { toJS } from 'mobx';


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

export const Default = Template.bind({});
Default.args = {
  page: new MangaPage()
};
