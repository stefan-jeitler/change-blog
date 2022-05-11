import {Component, OnInit} from '@angular/core';
import {TranslationKey} from "../../generated/TranslationKey";

@Component({
  selector: 'app-apikey',
  templateUrl: './apikey.component.html',
  styleUrls: ['./apikey.component.scss']
})
export class ApikeyComponent implements OnInit {

  constructor(public translationKey: TranslationKey) {
  }

  ngOnInit(): void {
  }

}
