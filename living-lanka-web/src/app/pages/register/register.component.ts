import { Component, inject, signal } from '@angular/core';
import { Router, RouterLink } from '@angular/router';
import { FormsModule } from '@angular/forms';
import { HttpErrorResponse } from '@angular/common/http';
import { AuthService } from '../../core/services/auth.service';
import { ProfileService } from '../../core/services/profile.service';

@Component({
  selector: 'app-register',
  imports: [RouterLink, FormsModule],
  template: `
    <div class="relative flex min-h-[70vh] items-center justify-center overflow-hidden px-4 py-16">
      <div class="absolute inset-0 bg-gradient-to-br from-maroon-950 via-maroon-900 to-black"></div>
      <div class="absolute inset-0 opacity-15 bg-hero-property bg-cover bg-center"></div>

      <div class="premium-card relative w-full max-w-lg p-8 sm:p-10">
        <h1 class="text-2xl font-bold text-maroon-900">Create Account</h1>
        <p class="mt-1 text-sm text-gray-500">Join Sri Lanka's premium marketplace</p>

        @if (error()) {
          <div class="alert alert-error mt-4 text-sm">{{ error() }}</div>
        }

        <form class="mt-6 space-y-4" (ngSubmit)="onSubmit()">
          <div class="grid grid-cols-1 gap-4 sm:grid-cols-2">
            <div class="form-control">
              <label class="label"><span class="label-text font-medium">First Name</span></label>
              <input type="text" [(ngModel)]="firstName" name="firstName" required class="input input-bordered w-full" />
            </div>
            <div class="form-control">
              <label class="label"><span class="label-text font-medium">Last Name</span></label>
              <input type="text" [(ngModel)]="lastName" name="lastName" required class="input input-bordered w-full" />
            </div>
          </div>
          <div class="form-control">
            <label class="label"><span class="label-text font-medium">Email</span></label>
            <input type="email" [(ngModel)]="email" name="email" required class="input input-bordered w-full" />
          </div>
          <div class="form-control">
            <label class="label"><span class="label-text font-medium">Phone</span></label>
            <input type="tel" [(ngModel)]="phoneNumber" name="phone" class="input input-bordered w-full" placeholder="0771234567" />
          </div>
          <div class="form-control">
            <label class="label"><span class="label-text font-medium">Password</span></label>
            <input type="password" [(ngModel)]="password" name="password" required minlength="6" class="input input-bordered w-full" />
          </div>
          <button
            type="submit"
            class="btn btn-block bg-gradient-to-r from-maroon-800 to-maroon-950 text-white"
            [disabled]="submitting()"
          >
            @if (submitting()) {
              <span class="loading loading-spinner loading-sm"></span>
            } @else {
              Create Account
            }
          </button>
        </form>

        <p class="mt-6 text-center text-sm text-gray-500">
          Already have an account?
          <a routerLink="/login" class="font-semibold text-maroon-800 hover:underline">Sign In</a>
        </p>
      </div>
    </div>
  `,
})
export class RegisterComponent {
  private readonly auth = inject(AuthService);
  private readonly profileService = inject(ProfileService);
  private readonly router = inject(Router);

  firstName = '';
  lastName = '';
  email = '';
  phoneNumber = '';
  password = '';
  readonly error = signal('');
  readonly submitting = signal(false);

  onSubmit(): void {
    this.error.set('');
    this.submitting.set(true);

    this.auth
      .register({
        email: this.email,
        password: this.password,
        firstName: this.firstName,
        lastName: this.lastName,
        phoneNumber: this.phoneNumber || undefined,
      })
      .subscribe({
        next: (res) => {
          this.profileService
            .create({
              userId: res.userId,
              firstName: this.firstName,
              lastName: this.lastName,
              phoneNumber: this.phoneNumber || undefined,
              currency: 'LKR',
              language: 'en',
              timezone: 'Asia/Colombo',
            })
            .subscribe({
              next: () => {
                this.auth.updateUserName(this.firstName, this.lastName);
                this.router.navigate(['/profile']);
              },
              error: () => this.router.navigate(['/']),
            });
        },
        error: (err: HttpErrorResponse) => {
          this.error.set(err.error?.error ?? 'Registration failed.');
          this.submitting.set(false);
        },
      });
  }
}
