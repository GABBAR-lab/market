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
      {
        path: 'search',
        loadComponent: () =>
          import('./pages/search/search.component').then((m) => m.SearchComponent),
      },
      {
        path: 'category/:slug',
        loadComponent: () =>
          import('./pages/category/category.component').then((m) => m.CategoryComponent),
      },
      {
        path: 'listing/:id',
        loadComponent: () =>
          import('./pages/listing-detail/listing-detail.component').then(
            (m) => m.ListingDetailComponent
          ),
      },
      {
        path: 'post-ad',
        canActivate: [authGuard],
        loadComponent: () =>
          import('./pages/post-ad/post-ad.component').then((m) => m.PostAdComponent),
      },
      {
        path: 'my-listings',
        canActivate: [authGuard],
        loadComponent: () =>
          import('./pages/my-listings/my-listings.component').then((m) => m.MyListingsComponent),
      },
      {
        path: 'profile',
        canActivate: [authGuard],
        loadComponent: () =>
          import('./pages/profile/profile.component').then((m) => m.ProfileComponent),
      },
      {
        path: 'login',
        loadComponent: () =>
          import('./pages/login/login.component').then((m) => m.LoginComponent),
      },
      {
        path: 'register',
        loadComponent: () =>
          import('./pages/register/register.component').then((m) => m.RegisterComponent),
      },
      {
        path: '**',
        loadComponent: () =>
          import('./pages/not-found/not-found.component').then((m) => m.NotFoundComponent),
      },
    ],
  },
];
