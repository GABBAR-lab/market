import { Component, inject, Input, OnInit, signal } from '@angular/core';
import { RouterLink } from '@angular/router';
import { CategoryIconComponent } from '../category-icon/category-icon.component';
import { ListingApiService } from '../../../core/services/listing-api.service';
import { CategoryTreeNode } from '../../../core/models/marketplace.models';
import { iconKeyFromCategory } from '../../../core/data/category-icons';

@Component({
  selector: 'app-category-accordion',
  imports: [RouterLink, CategoryIconComponent],
  template: `
    <div class="overflow-hidden rounded-lg border border-gray-200 bg-white">
      @if (loading()) {
        @for (i of [1,2,3,4,5,6]; track i) {
          <div class="skeleton h-14 border-b border-gray-100"></div>
        }
      } @else {
        @for (cat of tree(); track cat.id; let i = $index) {
          <div class="border-b border-gray-100 last:border-b-0">
            <button
              type="button"
              class="flex w-full items-center gap-3 px-4 py-3.5 text-left transition hover:bg-gray-50"
              (click)="toggle(cat.slug)"
            >
              <app-category-icon
                [iconKey]="parentIconKey(cat)"
                [slug]="cat.slug"
                size="md"
              />
              <span class="flex-1 text-base font-medium text-[#0074ba]">{{ cat.name }}</span>
              <svg
                class="h-4 w-4 shrink-0 text-gray-400 transition"
                [class.rotate-180]="expanded() === cat.slug"
                fill="none"
                stroke="currentColor"
                viewBox="0 0 24 24"
              >
                <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M19 9l-7 7-7-7" />
              </svg>
            </button>

            @if (expanded() === cat.slug && cat.subCategories.length) {
              <div class="bg-gray-50 pb-1">
                @for (sub of cat.subCategories; track sub.id) {
                  <a
                    [routerLink]="searchLink(sub, cat)"
                    [queryParams]="searchParams(sub, cat)"
                    class="flex items-center gap-3 border-t border-gray-100 px-4 py-3 pl-6 text-[#0074ba] hover:bg-gray-100"
                  >
                    @if (sub.iconUrl && sub.iconUrl !== 'all') {
                      <app-category-icon
                        [iconKey]="sub.iconUrl"
                        [slug]="sub.slug"
                        size="sm"
                      />
                    } @else {
                      <span class="w-5"></span>
                    }
                    <span class="text-sm">{{ sub.name }}</span>
                  </a>
                }
              </div>
            }
          </div>
        }
      }
    </div>
  `,
})
export class CategoryAccordionComponent implements OnInit {
  private readonly api = inject(ListingApiService);

  @Input() initialExpanded = '';

  readonly tree = signal<CategoryTreeNode[]>([]);
  readonly loading = signal(true);
  readonly expanded = signal('');

  ngOnInit(): void {
    if (this.initialExpanded) {
      this.expanded.set(this.initialExpanded);
    }

    this.api.getCategoryTree().subscribe({
      next: (nodes) => {
        this.tree.set(nodes);
        this.loading.set(false);
        if (!this.expanded() && nodes.length) {
          this.expanded.set(nodes[0].slug);
        }
      },
      error: () => this.loading.set(false),
    });
  }

  toggle(slug: string): void {
    this.expanded.set(this.expanded() === slug ? '' : slug);
  }

  parentIconKey(cat: CategoryTreeNode): string {
    return iconKeyFromCategory(cat.slug, cat.iconUrl);
  }

  searchLink(sub: CategoryTreeNode, parent: CategoryTreeNode): string[] {
    return sub.iconUrl === 'all' || sub.slug.startsWith('all-')
      ? ['/category', parent.slug]
      : ['/search'];
  }

  searchParams(sub: CategoryTreeNode, parent: CategoryTreeNode): Record<string, string> {
    if (sub.iconUrl === 'all' || sub.slug.startsWith('all-')) {
      return {};
    }

    return {
      category: parent.slug,
      query: sub.searchTerm ?? sub.name,
    };
  }
}
