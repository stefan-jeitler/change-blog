export type LengthUnit = 'px' | 'rem' | 'em' | '%' | 'vw' | 'vh';

export interface Length {
  value: number,
  unit: LengthUnit
}

export class Constants {
  public static readonly MobileBreakpoint: Length = {
    value: 768,
    unit: 'px'
  };
}
