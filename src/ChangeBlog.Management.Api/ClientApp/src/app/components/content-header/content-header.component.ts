import {Component, Input, OnInit} from '@angular/core';
import {Router} from "@angular/router";
import {Location} from "@angular/common";

@Component({
    selector: 'app-content-header',
    templateUrl: './content-header.component.html',
    styleUrls: ['./content-header.component.scss']
})
export class ContentHeaderComponent implements OnInit {

    @Input() title: string;
    @Input() showBackButton: boolean;
    @Input() goBackUrl: string;

    constructor(private router: Router,
                private location: Location) {
        this.title = '';
        this.showBackButton = false;
        this.goBackUrl = '';
    }

    ngOnInit(): void {
    }

    async goBack() {
        if (!!this.goBackUrl)
            await this.router.navigateByUrl(this.goBackUrl);
        else
            this.location.back();
    }
}
