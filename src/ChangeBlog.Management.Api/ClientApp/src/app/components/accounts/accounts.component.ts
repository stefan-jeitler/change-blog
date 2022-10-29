import { Component, OnInit } from '@angular/core';
import {TranslationKey} from "../../generated/TranslationKey";

@Component({
  selector: 'app-accounts',
  templateUrl: './accounts.component.html',
  styleUrls: ['./accounts.component.scss']
})
export class AccountsComponent implements OnInit {

  constructor(public translationKey: TranslationKey) { }

  ngOnInit(): void {
  }

}
