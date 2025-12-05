import { Routes } from '@angular/router';
import { CallbackComponent, FeedComponent, LoginComponent, mainRoutes } from './pages';
import path from 'path';

export const routes: Routes = [
  {
    path: '',
    children: mainRoutes,
  },
  {
    path: 'login',
    component: LoginComponent,
  },
  {
    path: 'callback',
    component: CallbackComponent,
  },
  {
    path: '**',
    redirectTo: '',
  }
];
