import { Routes } from '@angular/router';
import { authGuard } from '@core/guards/auth/auth.guard';
import { LayoutComponent } from './layout/layout.component';

export const mainRoutes: Routes = [
  {
    path: '',
    component: LayoutComponent, // LayoutComponent = Base page
    canActivate: [authGuard],
    children: [
      {
        path: '',
        redirectTo: 'feed',
        pathMatch: 'full',
      },
      {
        path: 'feed',
        loadComponent: () => import('./feed/feed.component').then((m) => m.FeedComponent),
      },
    ],
  },
];
