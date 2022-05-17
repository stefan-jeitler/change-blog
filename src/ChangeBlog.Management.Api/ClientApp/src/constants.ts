export type DistanceUnit = 'px' | 'rem' | 'em' | '%' | 'vw' | 'vh';

export interface Distance {
  value: number,
  unit: DistanceUnit
}

export class Constants {
  public static readonly MobileBreakpoint: Distance = {
    value: 768,
    unit: 'px'
  };
}
