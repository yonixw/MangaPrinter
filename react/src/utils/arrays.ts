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