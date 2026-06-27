import { Routes } from '@angular/router';
import { MainLayoutComponent } from './layout/main-layout/main-layout.component';
import { HomeComponent } from './pages/home/home.component';
import { authGuard } from './core/guards/auth.guard';

export const routes: Routes = [
  {
    path: '',
    component: MainLayoutComponent,
    children: [
      { path: '', component: HomeComponent },
      { path: 'categories', loadComponent: () => import('./pages/categories/categories.component').then((m) => m.CategoriesComponent) },
      { path: 'all-ads', loadComponent: () => import('./pages/all-ads/all-ads.component').then((m) => m.AllAdsComponent) },
      { path: 'search', loadComponent: () => import('./pages/search/search.component').then((m) => m.SearchComponent) },
      { path: 'saved', loadComponent: () => import('./pages/saved/saved.component').then((m) => m.SavedComponent) },
      { path: 'category/:slug', loadComponent: () => import('./pages/category/category.component').then((m) => m.CategoryComponent) },
      { path: 'listing/:id', loadComponent: () => import('./pages/listing-detail/listing-detail.component').then((m) => m.ListingDetailComponent) },
      { path: 'seller/:userId', loadComponent: () => import('./pages/seller/seller.component').then((m) => m.SellerComponent) },
      { path: 'post-ad', canActivate: [authGuard], loadComponent: () => import('./pages/post-ad/post-ad.component').then((m) => m.PostAdComponent) },
      { path: 'admin/pricing', canActivate: [authGuard], loadComponent: () => import('./pages/admin/admin-pricing.component').then((m) => m.AdminPricingComponent) },
      { path: 'my-listings', canActivate: [authGuard], loadComponent: () => import('./pages/my-listings/my-listings.component').then((m) => m.MyListingsComponent) },
      { path: 'profile', canActivate: [authGuard], loadComponent: () => import('./pages/profile/profile.component').then((m) => m.ProfileComponent) },
      { path: 'login', loadComponent: () => import('./pages/login/login.component').then((m) => m.LoginComponent) },
      { path: 'register', loadComponent: () => import('./pages/register/register.component').then((m) => m.RegisterComponent) },
      { path: 'about', loadComponent: () => import('./pages/static/static-pages.component').then((m) => m.AboutComponent) },
      { path: 'contact', loadComponent: () => import('./pages/static/static-pages.component').then((m) => m.ContactComponent) },
      { path: 'faq', loadComponent: () => import('./pages/static/static-pages.component').then((m) => m.FaqComponent) },
      { path: 'terms', loadComponent: () => import('./pages/static/static-pages.component').then((m) => m.TermsComponent) },
      { path: 'privacy', loadComponent: () => import('./pages/static/static-pages.component').then((m) => m.PrivacyComponent) },
      { path: 'sell-fast', loadComponent: () => import('./pages/static/static-pages.component').then((m) => m.SellFastComponent) },
      { path: 'membership', loadComponent: () => import('./pages/static/static-pages.component').then((m) => m.MembershipComponent) },
      { path: 'chat', loadComponent: () => import('./pages/static/static-pages.component').then((m) => m.ChatComponent) },
      { path: '**', loadComponent: () => import('./pages/not-found/not-found.component').then((m) => m.NotFoundComponent) },
    ],
  },
];
