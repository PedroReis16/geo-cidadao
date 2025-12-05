import { inject, Injectable } from '@angular/core';
import { HttpInterceptor, HttpRequest, HttpHandler, HttpEvent } from '@angular/common/http';
import { from, Observable, switchMap } from 'rxjs';
import { OauthService } from '@core/services/oauth/oauth.service';

@Injectable()
export class AuthInterceptor implements HttpInterceptor {
  oauthService = inject(OauthService);

  intercept(req: HttpRequest<any>, next: HttpHandler): Observable<HttpEvent<any>> {
    return from(this.oauthService.accessToken).pipe(
      switchMap((token) => {
        
        if (req.headers.get('Skip-Auth') === 'true') {
          const clonedRequest = req.clone({
            headers: req.headers.delete('Skip-Auth')
          });
          return next.handle(clonedRequest);
        }

        const clonedRequest = req.clone({
          setHeaders: {
            Authorization: `Bearer ${token}`
          }
        });

        return next.handle(clonedRequest);
      })
    );
  }
}