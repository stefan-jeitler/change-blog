import {Injectable} from "@angular/core";
import {HammerGestureConfig} from "@angular/platform-browser";
import * as Hammer from "hammerjs";

@Injectable()
export class SwiperConfig extends HammerGestureConfig {
  overrides = <any>{
    swipe: {direction: Hammer.DIRECTION_ALL},
  };
}