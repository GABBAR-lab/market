import { Component } from '@angular/core';
import { RouterOutlet } from '@angular/router';
import { TopBarComponent } from '../top-bar/top-bar.component';
import { SiteHeaderComponent } from '../site-header/site-header.component';
import { MainNavComponent } from '../main-nav/main-nav.component';
import { SiteFooterComponent } from '../site-footer/site-footer.component';
import { MobileBottomNavComponent } from '../mobile-bottom-nav/mobile-bottom-nav.component';
import { ToastContainerComponent } from '../../shared/components/toast-container/toast-container.component';
import { ScrollTopComponent } from '../../shared/components/scroll-top/scroll-top.component';

@Component({
  selector: 'app-main-layout',
  imports: [
    RouterOutlet,
    TopBarComponent,
    SiteHeaderComponent,
    MainNavComponent,
    SiteFooterComponent,
    MobileBottomNavComponent,
    ToastContainerComponent,
    ScrollTopComponent,
  ],
  template: `
    <div class="flex min-h-screen flex-col" data-theme="livinglanka">
      <app-top-bar />
      <app-site-header />
      <app-main-nav />
      <main class="flex-1 pb-safe-nav md:pb-0">
        <router-outlet />
      </main>
      <app-site-footer />
      <app-mobile-bottom-nav />
      <app-toast-container />
      <app-scroll-top />
    </div>
  `,
})
export class MainLayoutComponent {}
