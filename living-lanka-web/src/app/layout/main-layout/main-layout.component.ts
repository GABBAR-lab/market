import { Component } from '@angular/core';
import { RouterOutlet } from '@angular/router';
import { TopBarComponent } from '../top-bar/top-bar.component';
import { SiteHeaderComponent } from '../site-header/site-header.component';
import { MainNavComponent } from '../main-nav/main-nav.component';
import { SiteFooterComponent } from '../site-footer/site-footer.component';

@Component({
  selector: 'app-main-layout',
  imports: [
    RouterOutlet,
    TopBarComponent,
    SiteHeaderComponent,
    MainNavComponent,
    SiteFooterComponent,
  ],
  template: `
    <div class="flex min-h-screen flex-col" data-theme="livinglanka">
      <app-top-bar />
      <app-site-header />
      <app-main-nav />
      <main class="flex-1">
        <router-outlet />
      </main>
      <app-site-footer />
    </div>
  `,
})
export class MainLayoutComponent {}
