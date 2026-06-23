import { Component, inject, OnInit, signal } from '@angular/core';
import { RouterLink } from '@angular/router';
import { FormsModule } from '@angular/forms';
import { HttpErrorResponse } from '@angular/common/http';
import { AuthService } from '../../core/services/auth.service';
import { ProfileService } from '../../core/services/profile.service';
import { ProfileResponse } from '../../core/models/api.models';

@Component({
  selector: 'app-profile',
  imports: [RouterLink, FormsModule],
  template: `
    <div class="section-container py-12">
      <div class="mb-8">
        <p class="text-sm font-semibold uppercase tracking-widest text-gold-600">Account</p>
        <h1 class="text-3xl font-bold text-gray-900">My Profile</h1>
      </div>

      @if (loading()) {
        <div class="skeleton h-96 w-full max-w-2xl rounded-2xl"></div>
      } @else if (error()) {
        <div class="alert alert-error max-w-2xl">{{ error() }}</div>
      } @else if (profile(); as p) {
        <div class="grid grid-cols-1 gap-8 lg:grid-cols-3">
          <div class="premium-card p-6 text-center lg:col-span-1">
            <div class="mx-auto flex h-24 w-24 items-center justify-center rounded-full bg-gradient-to-br from-maroon-800 to-maroon-950 text-3xl font-bold text-gold-400">
              {{ p.firstName.charAt(0) }}{{ p.lastName.charAt(0) }}
            </div>
            <h2 class="mt-4 text-xl font-bold">{{ p.firstName }} {{ p.lastName }}</h2>
            <p class="text-sm text-gray-500">{{ auth.user()?.email }}</p>
            <span class="badge mt-3" [class]="p.status === 'Active' ? 'badge-success' : 'badge-warning'">{{ p.status }}</span>
            <div class="mt-6 space-y-2">
              <a routerLink="/my-listings" class="btn btn-outline btn-block border-maroon-800 text-maroon-800">My Listings</a>
              <a routerLink="/post-ad" class="btn btn-block bg-maroon-800 text-white">Post New Ad</a>
            </div>
          </div>

          <div class="premium-card p-6 lg:col-span-2">
            @if (saveMessage()) {
              <div class="alert alert-success mb-4 text-sm">{{ saveMessage() }}</div>
            }
            <form class="space-y-4" (ngSubmit)="onSave()">
              <div class="grid grid-cols-1 gap-4 sm:grid-cols-2">
                <div class="form-control">
                  <label class="label"><span class="label-text font-medium">First Name</span></label>
                  <input type="text" [(ngModel)]="firstName" name="firstName" class="input input-bordered w-full" />
                </div>
                <div class="form-control">
                  <label class="label"><span class="label-text font-medium">Last Name</span></label>
                  <input type="text" [(ngModel)]="lastName" name="lastName" class="input input-bordered w-full" />
                </div>
              </div>
              <div class="form-control">
                <label class="label"><span class="label-text font-medium">Phone</span></label>
                <input type="tel" [(ngModel)]="phoneNumber" name="phone" class="input input-bordered w-full" />
              </div>
              <div class="form-control">
                <label class="label"><span class="label-text font-medium">Bio</span></label>
                <textarea [(ngModel)]="bio" name="bio" class="textarea textarea-bordered h-24 w-full" placeholder="Tell buyers about yourself..."></textarea>
              </div>
              <div class="grid grid-cols-1 gap-4 sm:grid-cols-2">
                <div class="form-control">
                  <label class="label"><span class="label-text font-medium">Currency</span></label>
                  <select [(ngModel)]="currency" name="currency" class="select select-bordered w-full">
                    <option value="LKR">LKR</option>
                    <option value="USD">USD</option>
                  </select>
                </div>
                <div class="form-control">
                  <label class="label"><span class="label-text font-medium">Language</span></label>
                  <select [(ngModel)]="language" name="language" class="select select-bordered w-full">
                    <option value="en">English</option>
                    <option value="si">Sinhala</option>
                    <option value="ta">Tamil</option>
                  </select>
                </div>
              </div>
              <button type="submit" class="btn bg-maroon-800 text-white" [disabled]="saving()">
                @if (saving()) { <span class="loading loading-spinner loading-sm"></span> }
                Save Changes
              </button>
            </form>

            @if (p.addresses.length) {
              <div class="divider">Addresses</div>
              @for (addr of p.addresses; track addr.id) {
                <div class="rounded-xl bg-base-200 p-4 text-sm">
                  <p class="font-semibold">{{ addr.label }}</p>
                  <p class="text-gray-600">{{ addr.streetLine1 }}, {{ addr.city }}</p>
                </div>
              }
            }
          </div>
        </div>
      }
    </div>
  `,
})
export class ProfileComponent implements OnInit {
  readonly auth = inject(AuthService);
  private readonly profileService = inject(ProfileService);

  readonly profile = signal<ProfileResponse | null>(null);
  readonly loading = signal(true);
  readonly error = signal('');
  readonly saving = signal(false);
  readonly saveMessage = signal('');

  firstName = '';
  lastName = '';
  phoneNumber = '';
  bio = '';
  currency = 'LKR';
  language = 'en';

  ngOnInit(): void {
    this.profileService.getMyProfile().subscribe({
      next: (p) => {
        this.profile.set(p);
        this.firstName = p.firstName;
        this.lastName = p.lastName;
        this.phoneNumber = p.phoneNumber ?? '';
        this.bio = p.bio ?? '';
        this.currency = p.currency;
        this.language = p.language;
        this.auth.updateUserName(p.firstName, p.lastName);
        this.loading.set(false);
      },
      error: (err: HttpErrorResponse) => {
        if (err.status === 404) {
          this.error.set('Profile not found. Please complete registration.');
        } else {
          this.error.set(err.error?.error ?? 'Failed to load profile.');
        }
        this.loading.set(false);
      },
    });
  }

  onSave(): void {
    const p = this.profile();
    if (!p) return;

    this.saving.set(true);
    this.saveMessage.set('');

    this.profileService
      .update(p.id, {
        firstName: this.firstName,
        lastName: this.lastName,
        phoneNumber: this.phoneNumber || undefined,
        bio: this.bio || undefined,
        currency: this.currency,
        language: this.language,
      })
      .subscribe({
        next: (updated) => {
          this.profile.set(updated);
          this.auth.updateUserName(updated.firstName, updated.lastName);
          this.saveMessage.set('Profile updated successfully.');
          this.saving.set(false);
        },
        error: (err: HttpErrorResponse) => {
          this.error.set(err.error?.error ?? 'Failed to save profile.');
          this.saving.set(false);
        },
      });
  }
}
