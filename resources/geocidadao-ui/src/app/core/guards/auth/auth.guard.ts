import { inject } from '@angular/core';
import { CanActivateFn, Router } from '@angular/router';
import { OauthService } from '../../services';

export const authGuard: CanActivateFn = async () => {
  const router = inject(Router);
  const authService = inject(OauthService);
  const isAuthenticated = await authService.isAuthenticated();

  return isAuthenticated ? true : router.createUrlTree(['/login']);
};
