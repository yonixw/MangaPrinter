
 import React from 'react';
 // also exported from '@storybook/react' if you can deal with breaking changes in 6.1
 import { Story, Meta } from '@storybook/react/types-6-0';
 


import { PromptDialog, DialogResult, PromptDialogArgs } from './promptdialog';
import { Button, Menu } from 'antd';
 
 export default {
   title: 'Example/PromptDialog',
   component: PromptDialog,
   //argTypes: {
   //  backgroundColor: { control: 'color' },
   //},
 } as Meta;
 
 
 const Template: Story<PromptDialogArgs> 
    = (args) => <PromptDialog {...args} 
    onUpdate={(status,value)=>alert(status + ","+ value)} 
/>;

const buttonUI = (showDialog:()=>void)=>
   (
   <Button type="primary" onClick={showDialog}>
      Open Modal
   </Button>
   )
 

 export const Example1WithButton = Template.bind({});
 Example1WithButton.args = {
    title: "Example Title",
    desc: "Some description here",
    openUI: buttonUI
 };

 const { SubMenu } = Menu;

 const buttonUI2 = (showDialog:()=>void)=>
   (
   <Menu mode="horizontal">
   <Menu.Item>Menu</Menu.Item>
   <SubMenu title="SubMenu">
     <Menu.Item onClick={showDialog}>Open Dialog</Menu.Item>
   </SubMenu>
 </Menu>
   )
  
 export const DefaultValueExampleWithMenu = Template.bind({});
 DefaultValueExampleWithMenu.args = {
    title: "Example Title 2",
    desc: "Some description here",
    defaultValue: "Default value 👌",
    openUI: buttonUI2
 };