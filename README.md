# Management Product API

REST API untuk manajemen produk dengan authentication JWT, dibangun menggunakan .NET 10.

## Tech Stack

- .NET 10 (ASP.NET Core Web API)
- Entity Framework Core + SQLite
- JWT Bearer Authentication
- Memory Cache
- Rate Limiting (built-in ASP.NET Core)
- Docker & Docker Compose
- Swagger UI

---

## Persyaratan

Pastikan sudah terinstall salah satu dari:

| Mode | Requirement |
|------|-------------|
| Lokal | .NET 10 SDK |
| Docker | Docker Desktop |

---

## Cara Menjalankan

### A. Jalankan Lokal

1. Clone repository
```bash
   git clone 
   cd ManagementProduct
```

2. Restore dependencies
```bash
   dotnet restore
```

3. Jalankan aplikasi
```bash
   dotnet run
```

4. API berjalan di:
```
   http://localhost:7030  
```
atau sesuaikann dengan port di locall anda
---

### B. Jalankan dengan Docker

1. Clone repository
```bash
   git clone https://github.com/santo02/ManagementProductAPI.git
   cd ManagementProduct
```

2. Build dan jalankan container
```bash
   docker-compose up --build
```

3. API berjalan di:
```
   http://localhost:8080
```

---

## API Endpoints

### Auth

| Method | Endpoint | Auth | Rate Limit | Deskripsi |
|--------|----------|------|------------|-----------|
| POST | /api/auth/register | - | 3x/60s | Register user baru |
| POST | /api/auth/login | - | 3x/60s | Login & dapatkan token |

### Products

| Method | Endpoint | Auth | Rate Limit | Deskripsi |
|--------|----------|------|------------|-----------|
| GET | /api/products | - | - | Ambil semua produk |
| GET | /api/products/:id | - | - | Ambil produk by ID |
| POST | /api/products | ✅ JWT | 1x/5s | Buat produk baru |
| PUT | /api/products/:id | ✅ JWT | 1x/5s | Update produk |
| DELETE | /api/products/:id | ✅ JWT | 1x/5s | Hapus produk |

### Query Parameters (GET /api/products)

| Parameter | Contoh | Deskripsi |
|-----------|--------|-----------|
| search | ?search=shirt | Cari berdasarkan nama produk |
| category | ?category=Clothes | Filter berdasarkan kategori |
| page | ?page=1 | Nomor halaman (default: 1) |
| limit | ?limit=10 | Jumlah data per halaman (default: 10) |

-
## HTTP Status Codes

| Code | Keterangan |
|------|------------|
| 200 | OK - Request berhasil |
| 201 | Created - Data berhasil dibuat |
| 400 | Bad Request - Validasi gagal |
| 401 | Unauthorized - Token tidak valid atau expired |
| 404 | Not Found - Data tidak ditemukan |
| 429 | Too Many Requests - Rate limit terlampaui |

---

## API Documentation
### Import Postman Collection

1. Download file `ManagementProduct.postman_collection.json` dari repository ini
2. Buka Postman → klik **Import**
3. Pilih file tersebut → klik **Import**
4. Set variable `base_url` sesuai environment:
   - Lokal: `http://localhost:7030` atau sesuiakan dengan port lokal anda
   - Docker: `http://localhost:8080`

### Cara Menggunakan Token di Postman

1. Jalankan request **POST /api/auth/register** untuk daftar
2. Jalankan request **POST /api/auth/login**
3. Copy nilai `authenticationToken` dari response
4. Token otomatis tersimpan ke variable `{{token}}` via test script
5. Semua request yang butuh auth sudah terkonfigurasi menggunakan `{{token}}`