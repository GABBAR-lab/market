import { Injectable, inject } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable, from, map, switchMap } from 'rxjs';
import { environment } from '../../../environments/environment';

@Injectable({ providedIn: 'root' })
export class ImageUploadService {
  private readonly http = inject(HttpClient);

  uploadImages(files: File[]): Observable<string[]> {
    return from(this.convertAllToWebp(files)).pipe(
      switchMap((webpFiles) => {
        const form = new FormData();
        webpFiles.forEach((f) => form.append('files', f, f.name));
        return this.http.post<{ urls: string[] }>(`${environment.apiBaseUrl}/media/upload`, form);
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
      const url = URL.createObjectURL(file);
      img.onload = () => {
        URL.revokeObjectURL(url);
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
        URL.revokeObjectURL(url);
        reject(new Error(`Invalid image: ${file.name}`));
      };
      img.src = url;
    });
  }
}
