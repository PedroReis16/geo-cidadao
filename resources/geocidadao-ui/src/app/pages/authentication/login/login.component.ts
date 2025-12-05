import { Component, inject, OnInit } from '@angular/core';
import { Router } from '@angular/router';
import { OauthService } from '@core/services';

@Component({
  selector: 'app-login',
  imports: [],
  templateUrl: './login.component.html',
  styleUrl: './login.component.css',
})
export class LoginComponent implements OnInit {
  oauthService = inject(OauthService);
  router = inject(Router);

  async ngOnInit(): Promise<void> {
    if(await this.oauthService.isAuthenticated()) {
      this.router.navigate(['/']);
    }

    this.oauthService.login();
  }
}
