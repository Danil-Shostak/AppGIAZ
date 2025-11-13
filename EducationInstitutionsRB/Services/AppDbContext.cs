using EducationInstitutionsRB.Models;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using System;
using System.IO;
using System.Linq;

namespace EducationInstitutionsRB.Services;

public class AppDbContext : DbContext
{
    public DbSet<Region> Regions { get; set; }
    public DbSet<District> Districts { get; set; }
    public DbSet<Institution> Institutions { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        try
        {
            var databasePath = Path.Combine(Windows.Storage.ApplicationData.Current.LocalFolder.Path, "education.db");
            var connectionString = $"Data Source={databasePath}";

            optionsBuilder.UseSqlite(connectionString);

            System.Diagnostics.Debug.WriteLine($"Database path: {databasePath}");
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"Error configuring database: {ex.Message}");
            throw;
        }
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        // Настройка отношений
        modelBuilder.Entity<District>()
            .HasOne(d => d.Region)
            .WithMany()
            .HasForeignKey(d => d.RegionId);

        modelBuilder.Entity<Institution>()
            .HasOne(i => i.District)
            .WithMany()
            .HasForeignKey(i => i.DistrictId);
    }

    public void InitializeDatabase()
    {
        try
        {
            // Создаем базу данных и таблицы
            Database.EnsureCreated();
            System.Diagnostics.Debug.WriteLine("Database created successfully");
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"Error creating database: {ex.Message}");
            // Пробуем альтернативный метод
            CreateDatabaseManually();
        }
    }

    private void CreateDatabaseManually()
    {
        try
        {
            var databasePath = Path.Combine(Windows.Storage.ApplicationData.Current.LocalFolder.Path, "education.db");
            var connectionString = $"Data Source={databasePath}";

            using var connection = new SqliteConnection(connectionString);
            connection.Open();

            // Создаем таблицы вручную
            var command = connection.CreateCommand();
            command.CommandText = @"
                CREATE TABLE IF NOT EXISTS Regions (
                    Id INTEGER PRIMARY KEY AUTOINCREMENT,
                    Name TEXT NOT NULL
                );

                CREATE TABLE IF NOT EXISTS Districts (
                    Id INTEGER PRIMARY KEY AUTOINCREMENT,
                    Name TEXT NOT NULL,
                    RegionId INTEGER NOT NULL,
                    FOREIGN KEY (RegionId) REFERENCES Regions(Id)
                );

                CREATE TABLE IF NOT EXISTS Institutions (
                    Id INTEGER PRIMARY KEY AUTOINCREMENT,
                    Name TEXT NOT NULL,
                    Type TEXT NOT NULL,
                    Address TEXT NOT NULL,
                    Contacts TEXT,
                    DistrictId INTEGER NOT NULL,
                    Status TEXT NOT NULL,
                    RegistrationDate TEXT NOT NULL,
                    StudentCount INTEGER NOT NULL DEFAULT 0,
                    AdmittedCount INTEGER NOT NULL DEFAULT 0,
                    ExpelledCount INTEGER NOT NULL DEFAULT 0,
                    StaffCount INTEGER NOT NULL DEFAULT 0,
                    FOREIGN KEY (DistrictId) REFERENCES Districts(Id)
                );
            ";

            command.ExecuteNonQuery();
            System.Diagnostics.Debug.WriteLine("Database created manually");
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"Error creating database manually: {ex.Message}");
        }
    }

    public void DetachAllEntities()
    {
        var changedEntriesCopy = ChangeTracker.Entries()
            .Where(e => e.State == EntityState.Added ||
                        e.State == EntityState.Modified ||
                        e.State == EntityState.Deleted)
            .ToList();

        foreach (var entry in changedEntriesCopy)
            entry.State = EntityState.Detached;
    }
}