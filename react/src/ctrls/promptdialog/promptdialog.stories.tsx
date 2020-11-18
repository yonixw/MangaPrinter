
 import React from 'react';
 // also exported from '@storybook/react' if you can deal with breaking changes in 6.1
 import { Story, Meta } from '@storybook/react/types-6-0';
 


import { PromptDialog, DialogResult } from './promptdialog';
 
 export default {
   title: 'Example/PromptDialog',
   component: PromptDialog,
   //argTypes: {
   //  backgroundColor: { control: 'color' },
   //},
 } as Meta;
 
 const Template: Story<{title:string, desc:string, defaultValue:string, onUpdate:DialogResult}> 
    = (args) => <PromptDialog {...args} 
    onUpdate={(status,value)=>alert(status + ","+ value)} 
/>;
 
 export const Example1 = Template.bind({});
 Example1.args = {
    title: "Example Title",
    desc: "Some description here"
 };
  
 export const DefaultValueExample = Template.bind({});
 DefaultValueExample.args = {
    title: "Example Title 2",
    desc: "Some description here",
    defaultValue: "Default value 👌",
 };