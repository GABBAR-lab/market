import { Component, Input } from '@angular/core';
import { getCategoryIcon, iconKeyFromCategory } from '../../../core/data/category-icons';

@Component({
  selector: 'app-category-icon',
  template: `
    <svg
      [attr.viewBox]="def.viewBox ?? '0 0 24 24'"
      [class]="sizeClass"
      aria-hidden="true"
    >
      @for (shape of def.shapes; track $index) {
        <path
          [attr.d]="shape.d"
          [attr.fill]="shape.fill ?? 'none'"
          [attr.stroke]="shape.stroke"
          [attr.stroke-width]="shape.strokeWidth"
          [attr.fill-rule]="shape.fillRule"
        />
      }
    </svg>
  `,
})
export class CategoryIconComponent {
  @Input({ required: true }) iconKey = '';
  @Input() slug = '';
  @Input() size: 'sm' | 'md' | 'lg' = 'md';

  get def() {
    const key = this.iconKey || iconKeyFromCategory(this.slug, this.iconKey);
    return getCategoryIcon(key);
  }

  get sizeClass(): string {
    switch (this.size) {
      case 'sm':
        return 'h-5 w-5 shrink-0';
      case 'lg':
        return 'h-8 w-8 shrink-0';
      default:
        return 'h-6 w-6 shrink-0';
    }
  }
}
