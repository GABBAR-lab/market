# Living Lanka — Run Guide

## Ek Command Se Poora Project Chalao

Project root (`d:\marketplace-main`) mein:

```powershell
npm start
```

Ya double-click: **`start.bat`**

Yeh automatically start karega:
- Identity API → port 5001
- Profile API → port 5002
- Listing API → port 5003
- Frontend → port 4200

Browser: **http://localhost:4200**

---

## Pehli Baar Setup

### 1. SQL Server chal raha ho

Windows par usually **SQL Server Express** ya **LocalDB** already hota hai (Visual Studio ke sath).

Check karo:
```powershell
sqlcmd -S localhost -E -Q "SELECT @@VERSION"
```

Agar error aaye, Docker se SQL chalao:
```powershell
npm run db:docker
```
Phir connection strings update karo (neeche dekho).

### 2. .NET SDK + Node.js

```powershell
dotnet --version
npm --version
```

---

## Database Mein Demo Data

Jab services pehli baar start hoti hain, **automatic seed** hota hai:

| Database | Demo Data |
|----------|-----------|
| Identity | Admin + Seller users |
| Profile | Admin + Seller profiles |
| Listing | 16 categories, locations, **8 sample ads** |

### Test Accounts

| Role | Email | Password |
|------|-------|----------|
| Admin | `admin@marketplace.com` | `Admin@123` |
| Seller | `seller@marketplace.com` | `Seller@123` |

### Fresh Data (database reset)

Pehle stop karo, phir:
```powershell
npm run stop
npm run db:reset
```
Type `YES` jab pooche. Phir:
```powershell
npm start
```

---

## Useful Commands

| Command | Kaam |
|---------|------|
| `npm start` | Sab kuch start |
| `npm run stop` | Sab band |
| `npm run health` | Check services chal rahi hain |
| `npm run db:reset` | Database wipe + fresh seed |
| `npm run db:docker` | SQL Server Docker mein |

---

## URLs

| Service | URL |
|---------|-----|
| **Frontend** | http://localhost:4200 |
| Identity Swagger | http://localhost:5001/swagger |
| Profile Swagger | http://localhost:5002/swagger |
| Listing Swagger | http://localhost:5003/swagger |

---

## Docker SQL (optional)

Agar LocalDB nahi hai, `docker-compose.yml` use karo:

```powershell
npm run db:docker
```

Connection string har service ke `appsettings.json` mein:
```
Server=localhost,1433;Database=...;User Id=sa;Password=MarketPlace@123;TrustServerCertificate=True;
```

---

## Troubleshooting

| Problem | Hal |
|---------|-----|
| Frontend par data nahi | `npm run health` — 3 backend pehle start hon |
| SQL error | SQL Server service start karo |
| Purana data, naye ads nahi | `npm run db:reset` phir `npm start` |
| Port busy | `npm run stop` |
