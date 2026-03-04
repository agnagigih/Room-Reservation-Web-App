# Room Booking System — Dokumentasi

## Daftar Isi
1. [Gambaran Umum](#gambaran-umum)
2. [Teknologi yang Digunakan](#teknologi)
3. [Struktur Proyek](#struktur-proyek)
4. [Prasyarat Instalasi](#prasyarat)
5. [Cara Menjalankan Aplikasi](#cara-menjalankan)
6. [Konfigurasi Database](#konfigurasi-database)
7. [Fitur Aplikasi](#fitur-aplikasi)
8. [Akun Default](#akun-default)
9. [Panduan Penggunaan](#panduan-penggunaan)

---

## 1. Gambaran Umum

Room Booking System adalah aplikasi web berbasis **ASP.NET Core 8 MVC** untuk manajemen booking ruangan meeting secara digital. Sistem mendukung dua role user:

- **Admin** — dapat mengelola user, ruangan, dan semua booking (approve/reject)
- **User** — dapat membuat dan memantau booking miliknya sendiri

---

## 2. Teknologi yang Digunakan

| Komponen | Detail |
|---|---|
| **Framework** | ASP.NET Core 8.0 MVC |
| **ORM** | Entity Framework Core 8.0 |
| **Database** | Microsoft SQL Server (LocalDB untuk dev) |
| **Autentikasi** | Cookie Authentication (.NET Identity-style) |
| **Password Hashing** | BCrypt.Net-Next |
| **Frontend** | Bootstrap 5.3, Font Awesome 6.5 |
| **Pattern** | MVC (Model-View-Controller) |

---

## 3. Struktur Proyek

```
RoomBooking/
├── Controllers/
│   ├── AccountController.cs        # Controller Login, Logout
│   ├── HomeController.cs           # Controller Dashboard
│   ├── UserController.cs           # Controller User (Admin only)
│   ├── RoomController.cs           # Controller Ruangan
│   └── BookingController.cs        # Controller Booking + Approve/Reject
├── Models/
│   ├── Booking.cs                  # Entity: Booking
│   ├── Room.cs                     # Entity: Room
│   └── User.cs                     # Entity: User
├── Data/
│   └── AppDbContext.cs             # DbContext + Seed Data
├── ViewModels/
│   ├── BookingCreateViewModel.cs   # ViewModel untuk form Create Booking
│   ├── BookingEditViewModel.cs     # ViewModel untuk form Edit Booking
│   ├── BookingFilterViewModel.cs   # ViewModel untuk form Filter Booking
│   ├── UserCreateViewModel.cs      # ViewModel untuk form Create User
│   └── UserEditViewModel.cs        # ViewModel untuk form Edit User
├── Views/
│   ├── Shared/_Layout.cshtml       # Layout utama + Sidebar
│   ├── Account/Login.cshtml
│   ├── Home/Index.cshtml           # Dashboard
│   ├── Booking/                    # Index, Create, Edit, Detail
│   ├── Room/                       # Index, Create, Edit, Detail
│   └── User/                       # Index, Create, Edit, Detail
├── Services/
│   ├── AuthServices/
│   │   ├── AuthService.cs          # CRUD Login, Logout
│   │   └── IAuthService.cs         # Interface Auth Service
│   ├── BookingServices/
│   │   ├── BookingService.cs       # CRUD Booking + Approve/Reject
│   │   └── IBookingService.cs      # Interface Booking Service
│   ├── RoomServices/ 
│   │   ├── IRoomService.cs         # Interface Room Service
│   │   └── RoomService.cs          # CRUD Ruangan
│   └── UserServices/ 
│       ├── IUserService.cs         # Interface User Service
│       └── UserService.cs          # CRUD User (Admin only)
├── appsettings.json                # Konfigurasi + Connection String
├── Program.cs                      # Entry point + DI + Middleware
├── Database_Script.sql             # Script SQL manual 
└── RoomBooking.csproj
```

---

## 4. Prasyarat Instalasi

Pastikan software berikut sudah terinstal:

| Software | Versi Minimum | Link Download |
|---|---|---|
| **.NET SDK** | 8.0+ | https://dotnet.microsoft.com/download |
| **SQL Server** | 2019+ atau LocalDB | https://www.microsoft.com/en-us/sql-server/sql-server-downloads |
| **Visual Studio** | 17.8+ | https://visualstudio.microsoft.com/ |
| **(Opsional) VS Code** | Latest | https://code.visualstudio.com/ |
| **(Opsional) SSMS** | 19+ | Untuk melihat database via GUI |

> **Cek versi .NET:**
> ```bash
> dotnet --version
> # Output harus: 8.0.x
> ```

---

## 5. Cara Menjalankan Aplikasi

### Metode A: Visual Studio (Rekomendasi)

1. **Buka proyek**
   ```
   File → Open → Project/Solution → pilih RoomBooking.csproj
   ```

2. **Restore NuGet Packages** (otomatis, atau manual):
   ```
   Tools → NuGet Package Manager → Restore NuGet Packages
   ```

3. **Konfigurasi Connection String** di `appsettings.json`:
   ```json
   "ConnectionStrings": {
        "DefaultConnection": "Server=localhost;Database=RoomBookingDB;Trusted_Connection=True;MultipleActiveResultSets=true;TrustServerCertificate=True"
    }
   ```


4. **Jalankan Migration** (buka Package Manager Console):
   ```
   Tools → NuGet Package Manager → Package Manager Console
   ```
   Ketik perintah:
   ```powershell
   Add-Migration InitialCreate
   Update-Database
   ```
   ini akan mengenerate table dan data yang diperlukan

5. **Jalankan Aplikasi:**
   - Tekan `F5` (dengan debug) atau `Ctrl+F5` (tanpa debug)
   - Browser akan otomatis terbuka ke `https://localhost:xxxx`

---

### Metode B: .NET CLI (Command Line)

1. **Clone / extract** proyek ke folder lokal

2. **Masuk ke folder proyek:**
   ```bash
   cd RoomBooking
   ```

3. **Restore dependencies:**
   ```bash
   dotnet restore
   ```

4. **Install EF Core Tools** (jika belum):
   ```bash
   dotnet tool install --global dotnet-ef
   ```

5. **Jalankan Migration:**
   ```bash
   dotnet ef migrations add InitialCreate
   dotnet ef database update
   ```
   ini akan mengenerate table dan data yang diperlukan

6. **Jalankan aplikasi:**
   ```bash
   dotnet run
   ```
   Akses di browser: `https://localhost:5001` atau `http://localhost:5000`

---

### Metode C: Script SQL Manual (Tanpa Migration)

Jika ingin membuat database secara manual tanpa EF Migration:

1. Buka **SQL Server Management Studio (SSMS)**
2. Connect ke SQL Server instance Anda
3. Buka file `Database_Script.sql`
4. Jalankan script (**F5** atau klik Execute)
5. Update `appsettings.json` sesuai connection string SQL Server Anda
6. Jalankan aplikasi dengan `dotnet run` atau Visual Studio
7. Buka RoomBooking/Services/BookingServices/BookingService.cs cari method HasConflictAsync comment bagian dengan migration & Update-Database dan uncomment bagian tanpa migration

---

## 6. Konfigurasi Database

### Connection String (appsettings.json)

**LocalDB (default - development):**
```json
"Server=localhost;Database=RoomBookingDB;Trusted_Connection=True;MultipleActiveResultSets=true;TrustServerCertificate=True"
```

**SQL Server dengan Username/Password:**
```json
"Server=YOUR_SERVER;Database=RoomBookingDB;User Id=YourUserName;Password=YourPassword;TrustServerCertificate=True"
```

### Struktur Tabel

```
Users
├── Id (PK, INT, IDENTITY)
├── FullName (NVARCHAR 100)
├── Email (NVARCHAR 100, UNIQUE)
├── PasswordHash (NVARCHAR 255) — BCrypt hash
├── Role (NVARCHAR 20) — 'Admin' | 'User'
├── IsActive (BIT)
├── IsDeleted (BIT)
└── CreatedAt (DATETIME2)

Rooms
├── Id (PK, INT, IDENTITY)
├── Name (NVARCHAR 100, UNIQUE)
├── Location (NVARCHAR 50)
├── Capacity (INT)
├── Description (NVARCHAR 500, nullable)
├── IsActive (BIT)
└── CreatedAt (DATETIME2)

Bookings
├── Id (PK, INT, IDENTITY)
├── UserId (FK → Users.Id)
├── RoomId (FK → Rooms.Id)
├── BookingDate (DATE)
├── StartTime (VARCHAR 5) — format "HH:mm"
├── EndTime (VARCHAR 5) — format "HH:mm"
├── Purpose (NVARCHAR 200)
├── Status (NVARCHAR 20) — Pending|Approved|Rejected|Cancelled
├── Notes (NVARCHAR 500, nullable)
├── CreatedAt (DATETIME2)
└── UpdatedAt (DATETIME2, nullable)
```

---

## 7. Fitur Aplikasi

### Autentikasi
- Login dengan email & password
- Cookie-based authentication (session 8 jam)
- Role-based access control (Admin / User)
- Password hashing dengan BCrypt

### CRUD User (Admin Only)
| Fitur | Keterangan |
|---|---|
| Lihat daftar | Tampil semua user dengan search |
| Tambah user | Form dengan validasi email unik |
| Edit user | Update data + ganti password opsional |
| Detail user | Lihat riwayat booking user |
| Nonaktifkan | Menonaktifkan user (IsActive = false) |
| Hapus user  | Soft delete user (IsDeleted = true) |

### CRUD Ruangan
| Fitur | Keterangan |
|---|---|
| Lihat daftar | Card view semua ruangan |
| Tambah ruangan | Admin only |
| Edit ruangan | Admin only |
| Detail ruangan | Lihat jadwal booking ruangan |
| Nonaktifkan | Admin only, soft delete |

### CRUD Booking Ruangan
| Fitur | Keterangan |
|---|---|
| Filter by tanggal | Filter booking berdasarkan tanggal |
| Filter by ruangan | Dropdown filter ruangan |
| Filter by status | Pending/Approved/Rejected/Cancelled |
| Buat booking | Pilih ruangan, tanggal, jam, keperluan |
| Cek bentrok | Otomatis cek konflik jadwal |
| Edit booking | Hanya booking berstatus Pending |
| Detail booking | Informasi lengkap booking |
| Batalkan | User/Admin bisa batalkan booking |
| Approve/Reject | Admin only |
| Hapus permanen | Admin only |

---

## 8. Akun Default

Setelah `dotnet ef database update`, akun berikut tersedia:

| Email | Password | Role |
|---|---|---|
| admin@roombooking.com | Admin@123 | Admin |

> Admin dapat menambahkan user baru melalui menu **Kelola User → Tambah User**

---

## 9. Panduan Penggunaan

### Sebagai Admin

1. Login dengan akun admin
2. **Dashboard** — lihat statistik booking hari ini
3. **Kelola User** — tambah/edit/nonaktifkan user
4. **Daftar Ruangan** — tambah/edit ruangan
5. **Booking Ruangan** — lihat semua booking, filter by tanggal, approve/reject booking pending

### Sebagai User

1. Login dengan akun user
2. **Buat Booking** → pilih ruangan, tanggal, jam mulai/selesai, keperluan
3. Sistem otomatis mengecek apakah ruangan tersedia di jam tersebut
4. Booking berstatus **Pending** sampai disetujui Admin
5. User hanya bisa melihat booking miliknya sendiri

### Alur Booking
```
User buat booking → Status: PENDING
                         ↓
Admin review → APPROVE → Status: APPROVED ✓
            → REJECT  → Status: REJECTED ✗
                         ↓
User/Admin → CANCEL  → Status: CANCELLED
```

---

## Build untuk Production

```bash
dotnet publish -c Release -o ./publish
```

File hasil publish ada di folder `./publish`, siap deploy ke IIS atau Docker.

---


