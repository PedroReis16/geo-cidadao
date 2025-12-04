import { Component, inject, OnInit } from '@angular/core';
import { Router } from '@angular/router';
import { Oauth2Service } from '@core/services';

@Component({
  selector: 'app-login',
  imports: [],
  templateUrl: './login.component.html',
  styleUrl: './login.component.css'
})
export class LoginComponent implements OnInit {
  oauth2Service = inject(Oauth2Service);
  router = inject(Router);

  async ngOnInit(): Promise<void> {
    const user = await this.oauth2Service.getUser();
    if (user) {
      this.router.navigate(['/']);
      return
    }

    await this.oauth2Service.login();
  }
}