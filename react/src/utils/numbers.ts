export const round = // https://stackoverflow.com/a/11832950/1997873
     (num:number) => Math.round((num + Number.EPSILON) * 100) / 100