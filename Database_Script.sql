-- ============================================================
--  ROOM BOOKING SYSTEM - Database Script
--  Database: RoomBookingDB
--  DBMS    : Microsoft SQL Server 2019+
--  Created : 2026
-- ============================================================

USE master;
GO

-- 1. Drop database jika sudah ada
IF EXISTS (SELECT name FROM sys.databases WHERE name = N'RoomBookingDB')
BEGIN
    ALTER DATABASE RoomBookingDB SET SINGLE_USER WITH ROLLBACK IMMEDIATE;
    DROP DATABASE RoomBookingDB;
END
GO

-- 2. Create database
CREATE DATABASE RoomBookingDB
COLLATE Latin1_General_CI_AS;
GO

USE RoomBookingDB;
GO

-- ============================================================
--  TABLE: Users
-- ============================================================
CREATE TABLE Users (
    Id          INT            IDENTITY(1,1) NOT NULL,
    FullName    NVARCHAR(100)  NOT NULL,
    Email       NVARCHAR(100)  NOT NULL,
    PasswordHash NVARCHAR(255) NOT NULL,
    Role        NVARCHAR(20)   NOT NULL DEFAULT 'User',  -- 'Admin' | 'User'
    IsActive    BIT            NOT NULL DEFAULT 1,
    CreatedAt   DATETIME2(7)   NOT NULL DEFAULT GETDATE(),

    CONSTRAINT PK_Users        PRIMARY KEY (Id),
    CONSTRAINT UQ_Users_Email  UNIQUE (Email),
    CONSTRAINT CK_Users_Role   CHECK (Role IN ('Admin', 'User'))
);
GO

-- ============================================================
--  TABLE: Rooms
-- ============================================================
CREATE TABLE Rooms (
    Id          INT           IDENTITY(1,1) NOT NULL,
    Name        NVARCHAR(100) NOT NULL,
    Location    NVARCHAR(50)  NOT NULL,
    Capacity    INT           NOT NULL DEFAULT 1,
    Description NVARCHAR(500) NULL,
    IsActive    BIT           NOT NULL DEFAULT 1,
    CreatedAt   DATETIME2(7)  NOT NULL DEFAULT GETDATE(),

    CONSTRAINT PK_Rooms       PRIMARY KEY (Id),
    CONSTRAINT UQ_Rooms_Name  UNIQUE (Name),
    CONSTRAINT CK_Rooms_Capacity CHECK (Capacity > 0)
);
GO

-- ============================================================
--  TABLE: Bookings
-- ============================================================
CREATE TABLE Bookings (
    Id          INT            IDENTITY(1,1) NOT NULL,
    UserId      INT            NOT NULL,
    RoomId      INT            NOT NULL,
    BookingDate DATE           NOT NULL,
    StartTime   VARCHAR(5)     NOT NULL,   -- format HH:mm
    EndTime     VARCHAR(5)     NOT NULL,   -- format HH:mm
    Purpose     NVARCHAR(200)  NOT NULL,
    Status      NVARCHAR(20)   NOT NULL DEFAULT 'Pending',  -- Pending|Approved|Rejected|Cancelled
    Notes       NVARCHAR(500)  NULL,
    CreatedAt   DATETIME2(7)   NOT NULL DEFAULT GETDATE(),
    UpdatedAt   DATETIME2(7)   NULL,

    CONSTRAINT PK_Bookings          PRIMARY KEY (Id),
    CONSTRAINT FK_Bookings_Users    FOREIGN KEY (UserId)  REFERENCES Users(Id),
    CONSTRAINT FK_Bookings_Rooms    FOREIGN KEY (RoomId)  REFERENCES Rooms(Id),
    CONSTRAINT CK_Bookings_Status   CHECK (Status IN ('Pending','Approved','Rejected','Cancelled')),
    CONSTRAINT CK_Bookings_Time     CHECK (StartTime < EndTime)
);
GO

-- Index untuk performa pencarian booking berdasarkan tanggal dan ruangan
CREATE INDEX IX_Bookings_RoomDate ON Bookings (RoomId, BookingDate, StartTime);
CREATE INDEX IX_Bookings_UserId   ON Bookings (UserId);
CREATE INDEX IX_Bookings_Status   ON Bookings (Status);
GO

-- ============================================================
--  SEED DATA
-- ============================================================

-- Admin user (password: Admin@123)
INSERT INTO Users (FullName, Email, PasswordHash, Role, IsActive, IsDeleted, CreatedAt)
VALUES
    (N'Administrator',   N'admin@roombooking.com',  N'$2a$11$xxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxx', N'Admin', 1, 0, '2024-01-01'),
    (N'User 1',        N'user1@roombooking.com',   N'$2a$11$xxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxx', N'User',  1, 0, '2024-01-15'),
    (N'User 2',      N'user2@roombooking.com',   N'$2a$11$xxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxx', N'User',  1, 0, '2024-01-20');
GO

-- NOTE: PasswordHash di atas adalah placeholder.
-- Password sebenarnya di-hash oleh aplikasi menggunakan BCrypt.
-- Jalankan aplikasi untuk auto-seed dengan hash yang benar (via EF Migration seed).

-- Rooms
INSERT INTO Rooms (Name, Location, Capacity, Description, IsActive, CreatedAt)
VALUES
    (N'Ruang Rapat Utama',  N'Lantai 1', 20, N'Ruang rapat besar dengan proyektor & AC',          1, '2026-01-01'),
    (N'Ruang Meeting A',    N'Lantai 2', 10, N'Ruang meeting sedang dilengkapi TV 55 inch',        1, '2026-01-01'),
    (N'Ruang Training',     N'Lantai 3', 50, N'Ruang training kapasitas besar dengan sound system',1, '2026-01-01'),
    (N'Ruang Direksi',      N'Lantai 4',  8, N'Ruang meeting eksklusif untuk direksi',             1, '2026-01-01');
GO

-- Sample Bookings
INSERT INTO Bookings (UserId, RoomId, BookingDate, StartTime, EndTime, Purpose, Status, Notes, CreatedAt)
VALUES
    (1, 1, CAST(GETDATE() AS DATE),          '09:00', '11:00', N'Rapat Bulanan Tim IT',        N'Approved', NULL,            GETDATE()),
    (2, 2, CAST(GETDATE() AS DATE),          '13:00', '15:00', N'Review Desain Produk Q1',     N'Pending',  N'Butuh marker', GETDATE()),
    (3, 3, DATEADD(day, 1, GETDATE()),       '08:00', '12:00', N'Training Karyawan Baru',      N'Pending',  NULL,            GETDATE()),
    (1, 4, DATEADD(day, 2, GETDATE()),       '10:00', '12:00', N'Rapat Direksi Kuartal',       N'Approved', NULL,            GETDATE());
GO

-- ============================================================
--  STORED PROCEDURE: Cek ketersediaan ruangan (tidak digunakan jika sudah menggunakan entity framework)
-- ============================================================
CREATE OR ALTER PROCEDURE sp_CekKetersediaanRuangan
    @RoomId     INT,
    @Tanggal    DATE,
    @JamMulai   VARCHAR(5),
    @JamSelesai VARCHAR(5),
    @ExcludeBookingId INT = NULL
AS
BEGIN
    SET NOCOUNT ON;
    SELECT COUNT(*) AS JumlahBentrok
    FROM Bookings
    WHERE RoomId = @RoomId
      AND BookingDate = @Tanggal
      AND Status NOT IN ('Cancelled', 'Rejected')
      AND (@ExcludeBookingId IS NULL OR Id <> @ExcludeBookingId)
      AND StartTime < @JamSelesai
      AND EndTime   > @JamMulai;
END
GO

PRINT 'Database RoomBookingDB berhasil dibuat!';
PRINT 'Jalankan aplikasi .NET untuk auto-migrate dengan seed data BCrypt yang benar.';
GO
