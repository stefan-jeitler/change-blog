import {Injectable} from '@angular/core';
import {HttpEvent, HttpHandler, HttpInterceptor, HttpRequest} from '@angular/common/http';
import {Observable} from 'rxjs';
import {TranslocoService} from "@ngneat/transloco";
import {TranslocoLocaleService} from "@ngneat/transloco-locale";

@Injectable()
export class DefaultRequestHeadersInterceptor implements HttpInterceptor {
  constructor(private translationService: TranslocoService,
              private localeService: TranslocoLocaleService) {
  }

  intercept(httpRequest: HttpRequest<any>, next: HttpHandler): Observable<HttpEvent<any>> {
    const locale = this.localeService.getLocale();
    const lang = this.translationService.getActiveLang();

    const acceptLanguage = `${locale},${lang}`;

    return next.handle(httpRequest.clone({
      headers: httpRequest.headers.set('Accept-Language', acceptLanguage),
    }));
  }
}
