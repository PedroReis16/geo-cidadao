import { Component, inject, OnInit } from '@angular/core';
import { Router } from '@angular/router';
import { OauthService } from '@core/services';

@Component({
  selector: 'app-login',
  imports: [],
  templateUrl: './login.component.html',
  styleUrl: './login.component.css'
})
export class LoginComponent implements OnInit {
  oauthService = inject(OauthService);
  router = inject(Router);

  async ngOnInit(): Promise<void> {
    const user = await this.oauthService.getUser();
    if (user) {
      this.router.navigate(['/']);
      return
    }

    await this.oauthService.login();
  }
}