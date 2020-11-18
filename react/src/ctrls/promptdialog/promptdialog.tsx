import React, { useState} from 'react';
import { Button, Input } from 'antd';
import Modal from 'antd/lib/modal/Modal';

export type DialogResult = (sucess: boolean, value: string)=>void

export const PromptDialog =
     ({title, desc, defaultValue, onUpdate}:
         {title:string, desc:string, defaultValue:string, onUpdate?:DialogResult}) => {
    const [visible,setVisible] = useState(false);
    const [value, setValue] = useState(defaultValue);

    const handleOk = ()=> {
        setVisible(false);
        if (onUpdate)
            onUpdate(true, value);
    }

    const handleClose = ()=> {
        setVisible(false);
        if (onUpdate)
            onUpdate(false, value);
    }

    return (
        <>
          <Button type="primary" onClick={()=>setVisible(true)}>
            Open Modal
          </Button>
          <Modal
            title={'✍ ' + title}
            visible={visible}
            onOk={handleOk}
            onCancel={handleClose}
          >
            <p>{desc}</p>
            <Input 
                placeholder={defaultValue||"Enter text..."} 
                onChange={(e)=>setValue(e.target.value)} />
          </Modal>
        </>
    );
}
