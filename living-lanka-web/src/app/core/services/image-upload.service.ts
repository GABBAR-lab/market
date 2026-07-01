import { Injectable, inject } from '@angular/core';
import { Observable } from 'rxjs';
import { MediaApiService } from './media-api.service';

/** @deprecated Use MediaApiService directly */
@Injectable({ providedIn: 'root' })
export class ImageUploadService {
  private readonly media = inject(MediaApiService);

  uploadImages(files: File[]): Observable<string[]> {
    return this.media.uploadListingImages(files);
  }
}
