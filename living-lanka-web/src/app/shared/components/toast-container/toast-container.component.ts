import { Component, inject } from '@angular/core';
import { ToastService } from '../../../core/services/toast.service';

@Component({
  selector: 'app-toast-container',
  template: `
    <div
      class="pointer-events-none fixed inset-x-0 top-20 z-[100] flex flex-col items-center gap-2 px-4 sm:items-end sm:pr-6"
      aria-live="polite"
      aria-atomic="true"
    >
      @for (toast of toastService.toasts(); track toast.id) {
        <div
          class="toast-pro pointer-events-auto animate-slide-in flex max-w-sm items-center gap-3 rounded-xl px-4 py-3 shadow-elevated"
          [class.toast-pro-success]="toast.type === 'success'"
          [class.toast-pro-error]="toast.type === 'error'"
          [class.toast-pro-warning]="toast.type === 'warning'"
          [class.toast-pro-info]="toast.type === 'info'"
          role="status"
        >
          <span class="text-sm font-medium">{{ toast.message }}</span>
          <button
            type="button"
            class="ml-auto shrink-0 rounded-lg p-1 opacity-70 hover:opacity-100"
            (click)="toastService.dismiss(toast.id)"
            aria-label="Dismiss"
          >
            <svg class="h-4 w-4" fill="none" stroke="currentColor" viewBox="0 0 24 24">
              <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M6 18L18 6M6 6l12 12" />
            </svg>
          </button>
        </div>
      }
    </div>
  `,
})
export class ToastContainerComponent {
  readonly toastService = inject(ToastService);
}
