import {Injectable} from "@angular/core";
import {HammerGestureConfig} from "@angular/platform-browser";
import * as Hammer from "hammerjs";

@Injectable()
export class SwipeConfig extends HammerGestureConfig {
  overrides = <any>{
    swipe: {direction: Hammer.DIRECTION_HORIZONTAL},
    pinch: { enable: false },
    rotate: { enable: false }
  };

  options = <any>{
    touchAction: 'auto',
    cssProps: {
      userSelect: 'auto'
    }
  };
}
