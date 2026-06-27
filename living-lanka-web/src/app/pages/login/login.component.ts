import { Component, inject, signal } from '@angular/core';
import { Router, RouterLink, ActivatedRoute } from '@angular/router';
import { FormsModule } from '@angular/forms';
import { HttpErrorResponse } from '@angular/common/http';
import { AuthService } from '../../core/services/auth.service';

@Component({
  selector: 'app-login',
  imports: [RouterLink, FormsModule],
  template: `
    <div class="relative flex min-h-[70vh] items-center justify-center overflow-hidden px-4 py-16">
      <div class="absolute inset-0 bg-gradient-to-br from-maroon-950 via-maroon-900 to-black"></div>
      <div class="absolute inset-0 opacity-20 bg-hero-beach bg-cover bg-center"></div>

      <div class="premium-card relative w-full max-w-md p-8 sm:p-10">
        <div class="mb-6 text-center">
          <img src="/images/living-lanka-logo.png" alt="Living Lanka" class="mx-auto mb-4 h-20 w-auto object-contain" />
          <h1 class="text-2xl font-bold text-maroon-900">Welcome Back</h1>
          <p class="mt-1 text-sm text-gray-500">Sign in to manage your ads and profile</p>
        </div>

        @if (error()) {
          <div class="alert alert-error mb-4 text-sm">{{ error() }}</div>
        }

        <form class="space-y-4" (ngSubmit)="onSubmit()">
          <div class="form-control">
            <label class="label"><span class="label-text font-medium">Email</span></label>
            <input type="email" [(ngModel)]="email" name="email" required class="input input-bordered w-full" placeholder="you@email.com" />
          </div>
          <div class="form-control">
            <label class="label"><span class="label-text font-medium">Password</span></label>
            <input type="password" [(ngModel)]="password" name="password" required class="input input-bordered w-full" placeholder="••••••••" />
          </div>
          <button
            type="submit"
            class="btn btn-block bg-gradient-to-r from-maroon-800 to-maroon-950 text-white hover:from-maroon-900 hover:to-black"
            [disabled]="submitting()"
          >
            @if (submitting()) {
              <span class="loading loading-spinner loading-sm"></span>
            } @else {
              Sign In
            }
          </button>
        </form>

        <p class="mt-6 text-center text-sm text-gray-500">
          Don't have an account?
          <a routerLink="/register" class="font-semibold text-maroon-800 hover:underline">Register</a>
        </p>
      </div>
    </div>
  `,
})
export class LoginComponent {
  private readonly auth = inject(AuthService);
  private readonly router = inject(Router);
  private readonly route = inject(ActivatedRoute);

  email = '';
  password = '';
  readonly error = signal('');
  readonly submitting = signal(false);

  onSubmit(): void {
    this.error.set('');
    this.submitting.set(true);

    this.auth.login({ email: this.email, password: this.password }).subscribe({
      next: () => {
        const returnUrl = this.route.snapshot.queryParamMap.get('returnUrl') ?? '/';
        this.router.navigateByUrl(returnUrl);
      },
      error: (err: HttpErrorResponse) => {
        this.error.set(err.error?.error ?? 'Login failed. Check your credentials.');
        this.submitting.set(false);
      },
    });
  }
}
