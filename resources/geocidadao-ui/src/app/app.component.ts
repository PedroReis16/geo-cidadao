import { Component, inject, OnInit } from '@angular/core';
import { RouterOutlet } from '@angular/router';
import { ThemeService } from '@core/services';
import { injectPaletteVariables } from '@shared/theme/geocidadao.theme';

@Component({
  selector: 'app-root',
  imports: [RouterOutlet],
  templateUrl: './app.component.html',
  styleUrl: './app.component.css',
  standalone: true,
})
export class AppComponent implements OnInit {
  themeService = inject(ThemeService);

  ngOnInit(): void {
    this.themeService.setTheme(this.themeService.getBrowserTheme());

    injectPaletteVariables();
  }
}
