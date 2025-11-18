using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using VehicleService.Data.Entities;

namespace VehicleService.Data.Configurations;

public partial class VehicleDbContext : DbContext
{
    public VehicleDbContext()
    {
    }

    public VehicleDbContext(DbContextOptions<VehicleDbContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Vehicle> Vehicles { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see https://go.microsoft.com/fwlink/?LinkId=723263.
        => optionsBuilder.UseSqlServer("Server=localhost;Database=EvCoOwnership_VehicleDb;Trusted_Connection=True;TrustServerCertificate=True;");

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Vehicle>(entity =>
        {
            entity.HasKey(e => e.VehicleId).HasName("PK__Vehicle__476B54924EF11EC9");

            entity.ToTable("Vehicle");

            entity.HasIndex(e => e.LicensePlate, "UQ__Vehicle__026BC15CB2B0CE97").IsUnique();

            entity.HasIndex(e => e.Vin, "UQ__Vehicle__C5DF234C75089559").IsUnique();

            entity.Property(e => e.BatteryCapacityKwh).HasColumnType("decimal(10, 2)");
            entity.Property(e => e.ChargingPortType).HasMaxLength(50);
            entity.Property(e => e.Color).HasMaxLength(255);
            entity.Property(e => e.LicensePlate).HasMaxLength(255);
            entity.Property(e => e.Make).HasMaxLength(255);
            entity.Property(e => e.Model).HasMaxLength(255);
            entity.Property(e => e.PurchasePrice).HasColumnType("decimal(18, 2)");
            entity.Property(e => e.Vin)
                .HasMaxLength(255)
                .HasColumnName("VIN");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
