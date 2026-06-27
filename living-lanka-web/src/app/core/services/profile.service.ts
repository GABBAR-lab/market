import { Injectable, inject } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable, map } from 'rxjs';
import { environment } from '../../../environments/environment';
import {
  CreateProfileRequest,
  ProfileResponse,
  UpdateProfileRequest,
} from '../models/api.models';
import { PublicSellerProfile } from '../models/marketplace.models';

@Injectable({ providedIn: 'root' })
export class ProfileService {
  private readonly http = inject(HttpClient);
  private readonly base = `${environment.apiBaseUrl}/profiles`;

  getMyProfile(): Observable<ProfileResponse> {
    return this.http.get<ProfileResponse>(`${this.base}/me`);
  }

  getById(id: string): Observable<ProfileResponse> {
    return this.http.get<ProfileResponse>(`${this.base}/${id}`);
  }

  create(request: CreateProfileRequest): Observable<ProfileResponse> {
    return this.http.post<ProfileResponse>(this.base, request);
  }

  update(id: string, request: UpdateProfileRequest): Observable<ProfileResponse> {
    return this.http.put<ProfileResponse>(`${this.base}/${id}`, request);
  }

  getPublicSeller(userId: string): Observable<PublicSellerProfile> {
    return this.http
      .get<{
        userId: string;
        displayName: string;
        avatarUrl?: string;
        bio?: string;
        memberSince: string;
      }>(`${this.base}/public/seller/${userId}`)
      .pipe(
        map((p) => ({
          userId: p.userId,
          displayName: p.displayName,
          avatarUrl: p.avatarUrl,
          bio: p.bio,
          memberSince: p.memberSince,
        }))
      );
  }
}
