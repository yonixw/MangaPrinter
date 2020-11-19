import React, { useState} from 'react';
import {  Input } from 'antd';
import Modal from 'antd/lib/modal/Modal';

export type DialogResult = (sucess: boolean, value: string)=>void

export type PromptDialogArgs = 
  { 
    title:string, desc:string, defaultValue:string, 
    openUI:(showDialog:()=>void)=>JSX.Element,
    keepLast?:boolean,
    onUpdate?:DialogResult
  }

export const PromptDialog = (props:PromptDialogArgs) => {
    const [visible,setVisible] = useState(false);
    const [value, setValue] = useState(props.defaultValue);

    const handleOk = ()=> {
        setVisible(false);
        if (props.onUpdate) props.onUpdate(true, value);
    }

    const handleClose = ()=> {
        setVisible(false);
        if (props.onUpdate) props.onUpdate(false, value);
    }

    return (
        <>
          {props.openUI(()=>{
            if (!props.keepLast) setValue(""); 
            setVisible(true)
          })}
          <Modal
            title={'✍ ' + props.title}
            visible={visible}
            onOk={handleOk}
            onCancel={handleClose}
          >
            <p>{props.desc}</p>
            <Input 
                value={value}
                placeholder={props.defaultValue||"Enter text..."} 
                onChange={(e)=>setValue(e.target.value)} />
          </Modal>
        </>
    );
}
