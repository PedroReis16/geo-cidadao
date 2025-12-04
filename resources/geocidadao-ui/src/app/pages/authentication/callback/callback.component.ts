import { Component, OnInit, inject } from '@angular/core';
import { Router } from '@angular/router';
import { OauthService } from '@core/services';

@Component({
  selector: 'app-callback',
  imports: [],
  templateUrl: './callback.component.html',
  styleUrl: './callback.component.css'
})
export class CallbackComponent implements OnInit {
  oauth2Service = inject(OauthService);
  router = inject(Router);

  async ngOnInit(): Promise<void> {
    await this.oauth2Service.handleCallback();
    this.router.navigate(['/']);
  }
}