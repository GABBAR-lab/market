import { Component, HostListener, signal } from '@angular/core';

@Component({
  selector: 'app-scroll-top',
  template: `
    @if (visible()) {
      <button
        type="button"
        class="btn-scroll-top"
        (click)="scrollToTop()"
        aria-label="Scroll to top"
      >
        <svg class="h-5 w-5" fill="none" stroke="currentColor" viewBox="0 0 24 24">
          <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M5 10l7-7m0 0l7 7m-7-7v18" />
        </svg>
      </button>
    }
  `,
})
export class ScrollTopComponent {
  readonly visible = signal(false);

  @HostListener('window:scroll')
  onScroll(): void {
    this.visible.set(window.scrollY > 400);
  }

  scrollToTop(): void {
    window.scrollTo({ top: 0, behavior: 'smooth' });
  }
}
