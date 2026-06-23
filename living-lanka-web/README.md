# Living Lanka — Angular Frontend

Sri Lanka marketplace UI integrated with **Identity**, **Profile**, and **Listing** microservices.

## Quick Start (Full Stack)

### 1. Start Backend Services (3 terminals)

```powershell
# Terminal 1 — Identity Service (port 5001)
cd d:\marketplace-main\services\identity-service\src\IdentityService.Api
dotnet run

# Terminal 2 — Profile Service (port 5002)
cd d:\marketplace-main\services\profile-service\src\ProfileService.Api
dotnet run

# Terminal 3 — Listing Service (port 5003)
cd d:\marketplace-main\services\listing-service\src\ListingService.Api
dotnet run
```

### 2. Start Frontend

```powershell
cd d:\marketplace-main\living-lanka-web
npm install
npm start
```

Open **http://localhost:4200**

## Test Credentials

| Role  | Email                    | Password   |
|-------|--------------------------|------------|
| Admin | `admin@marketplace.com`  | `Admin@123`|

Register a new account via **/register** for buyer flow. Posting ads requires **Seller** role (admin can assign via API).

## API Integration Map

| Service  | Port | Frontend Routes via Proxy      |
|----------|------|--------------------------------|
| Identity | 5001 | `/api/auth`, `/api/users`      |
| Profile  | 5002 | `/api/profiles`                |
| Listing  | 5003 | `/api/listings`, `/api/categories`, `/api/locations` |

Dev proxy: `proxy.conf.json` routes `/api/*` to the correct microservice.

## Pages

| Route            | API Used                              | Auth Required |
|------------------|---------------------------------------|---------------|
| `/`              | categories, featured listings         | No            |
| `/search`        | listings search                       | No            |
| `/category/:slug`| categories, listings search           | No            |
| `/listing/:id`   | listing detail, view count            | No            |
| `/login`         | auth login                            | No            |
| `/register`      | auth register + profile create        | No            |
| `/profile`       | profiles/me, update                   | Yes           |
| `/my-listings`   | listings/me, submit, delete           | Yes           |
| `/post-ad`       | categories, locations, create listing | Yes           |

## Health Check Commands

```powershell
curl http://localhost:5001/api/health
curl http://localhost:5002/api/health
curl http://localhost:5003/api/health
```

## Postman Collection

Import `postman/MarketPlace-API.postman_collection.json` and `postman/MarketPlace-Local.postman_environment.json` for full API testing.

## Build for Production

```bash
npm run build
```

Output: `dist/living-lanka-web/`

## Tech Stack

- Angular 22 (standalone components)
- Tailwind CSS 3 + DaisyUI 4
- JWT auth with HTTP interceptor
- Responsive premium UI design
