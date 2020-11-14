import { Reducer, useReducer } from "react";

export function removeItemOnce<T>(arr: Array<T>,finder:(e:T)=>boolean ) {
    var index = arr.findIndex(finder)
    if (index > -1) {
      arr.splice(index, 1);
    }
    return arr;
}

export function removeItemAll<T>(arr: Array<T>,finder:(e:T)=>boolean ) {
    var i = 0;
    while (i < arr.length) {
      if (finder(arr[i])) {
        arr.splice(i, 1);
      } else {
        ++i;
      }
    }
    return arr;
  }


export enum RActions  {
  INSERT = "INSERT",
  REMOVE = "REMOVE",
  UPDATE = "UPDATE",
}

interface RActionPayload<S> {
  type: string,
  payload: S,
  finder?: (e:S)=>boolean
}

export function _useReduceArr<S>(reducer:Reducer<Array<S>,RActionPayload<S>>, initial:Array<S>) {
  return useReducer<Reducer<Array<S>,RActionPayload<S>>>(reducer, initial);
}

export function useReduceArr<S>(initial:Array<S>) {
    return _useReduceArr(ArrayReducer, initial);
}

export const ArrayReducer = <S>(state:Array<S>,msg:RActionPayload<S>):Array<S>=>{
  const oldArr = state;
  switch (msg.type) {

    case RActions.INSERT:
      return [...(oldArr||[]), msg.payload]

    case RActions.REMOVE:
      removeItemOnce(
        state,
        msg.finder || function (e) {return e===msg.payload}
      );
      return [...oldArr];


    default:
     return state;
  }
}

