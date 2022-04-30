export interface Distance {
  value: number,
  unit: string
}

export class Constants {
  public static readonly MobileBreakpoint: Distance = {
    value: 576,
    unit: 'px'
  };
}
