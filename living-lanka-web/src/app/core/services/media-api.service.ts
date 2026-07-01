import { Injectable, inject } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable, from, map, switchMap } from 'rxjs';
import { environment } from '../../../environments/environment';

export interface MediaAsset {
  id: string;
  ownerUserId: string;
  category: string;
  fileName: string;
  url: string;
  contentType: string;
  sizeBytes: number;
  createdAt: string;
}

@Injectable({ providedIn: 'root' })
export class MediaApiService {
  private readonly http = inject(HttpClient);
  private readonly base = `${environment.apiBaseUrl}/media`;

  getMyMedia(category?: string): Observable<MediaAsset[]> {
    const params = category ? { category } : undefined;
    return this.http.get<MediaAsset[]>(`${this.base}/me`, { params });
  }

  getAllMedia(page = 1, pageSize = 50): Observable<MediaAsset[]> {
    return this.http.get<MediaAsset[]>(`${this.base}/admin`, {
      params: { page: String(page), pageSize: String(pageSize) },
    });
  }

  deleteMedia(id: string): Observable<{ message: string }> {
    return this.http.delete<{ message: string }>(`${this.base}/${id}`);
  }

  uploadListingImages(files: File[]): Observable<string[]> {
    return this.uploadWithConversion(`${this.base}/upload`, files);
  }

  uploadAvatar(file: File): Observable<string> {
    return from(this.convertToWebp(file)).pipe(
      switchMap((webp) => {
        const form = new FormData();
        form.append('file', webp, webp.name);
        return this.http.post<{ urls: string[] }>(`${this.base}/avatars`, form);
      }),
      map((res) => res.urls[0])
    );
  }

  private uploadWithConversion(url: string, files: File[]): Observable<string[]> {
    return from(this.convertAllToWebp(files)).pipe(
      switchMap((webpFiles) => {
        const form = new FormData();
        webpFiles.forEach((f) => form.append('files', f, f.name));
        return this.http.post<{ urls: string[] }>(url, form);
      }),
      map((res) => res.urls)
    );
  }

  private async convertAllToWebp(files: File[]): Promise<File[]> {
    const out: File[] = [];
    for (const file of files) {
      out.push(await this.convertToWebp(file));
    }
    return out;
  }

  private convertToWebp(file: File, maxWidth = 1200, quality = 0.82): Promise<File> {
    return new Promise((resolve, reject) => {
      const img = new Image();
      const objectUrl = URL.createObjectURL(file);
      img.onload = () => {
        URL.revokeObjectURL(objectUrl);
        const scale = Math.min(1, maxWidth / img.width);
        const w = Math.round(img.width * scale);
        const h = Math.round(img.height * scale);
        const canvas = document.createElement('canvas');
        canvas.width = w;
        canvas.height = h;
        const ctx = canvas.getContext('2d');
        if (!ctx) {
          reject(new Error('Canvas not supported'));
          return;
        }
        ctx.drawImage(img, 0, 0, w, h);
        canvas.toBlob(
          (blob) => {
            if (!blob) {
              reject(new Error('Failed to convert image'));
              return;
            }
            resolve(new File([blob], `${file.name.replace(/\.\w+$/, '')}.webp`, { type: 'image/webp' }));
          },
          'image/webp',
          quality
        );
      };
      img.onerror = () => {
        URL.revokeObjectURL(objectUrl);
        reject(new Error(`Invalid image: ${file.name}`));
      };
      img.src = objectUrl;
    });
  }
}
