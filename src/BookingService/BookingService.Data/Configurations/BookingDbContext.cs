using System;
using System.Collections.Generic;
using BookingService.Data.Entities;
using Microsoft.EntityFrameworkCore;

namespace BookingService.Data.Configurations;

public partial class BookingDbContext : DbContext
{
    public BookingDbContext()
    {
    }

    public BookingDbContext(DbContextOptions<BookingDbContext> options)
        : base(options)
    {
    }

    public virtual DbSet<BookingCheckInOut> BookingCheckInOuts { get; set; }

    public virtual DbSet<VehicleBooking> VehicleBookings { get; set; }

    public virtual DbSet<VehicleCondition> VehicleConditions { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see https://go.microsoft.com/fwlink/?LinkId=723263.
        => optionsBuilder.UseSqlServer("Server=localhost;Database=EvCoOwnership_BookingDb;Trusted_Connection=True;TrustServerCertificate=True;");

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<BookingCheckInOut>(entity =>
        {
            entity.HasKey(e => e.CheckInOutId).HasName("PK__BookingC__AFE958923B19257D");

            entity.ToTable("BookingCheckInOut");

            entity.HasOne(d => d.Booking).WithMany(p => p.BookingCheckInOuts)
                .HasForeignKey(d => d.BookingId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_CheckInOut_Booking");
        });

        modelBuilder.Entity<VehicleBooking>(entity =>
        {
            entity.HasKey(e => e.BookingId).HasName("PK__VehicleB__73951AED772BBA37");

            entity.ToTable("VehicleBooking", tb => tb.HasTrigger("TR_Booking_NoOverlap"));

            entity.HasIndex(e => new { e.UserId, e.StartDate }, "IX_Booking_User_Time");

            entity.HasIndex(e => new { e.VehicleId, e.StartDate, e.EndDate }, "IX_Booking_Vehicle_Time");

            entity.Property(e => e.DistanceKm).HasColumnType("decimal(10, 2)");
            entity.Property(e => e.EnergyKwh).HasColumnType("decimal(10, 2)");
        });

        modelBuilder.Entity<VehicleCondition>(entity =>
        {
            entity.HasKey(e => e.VehicleConditionId).HasName("PK__VehicleC__78BAC83CD33F10F4");

            entity.ToTable("VehicleCondition");

            entity.Property(e => e.Name).HasMaxLength(255);

            entity.HasOne(d => d.CheckInOut).WithMany(p => p.VehicleConditions)
                .HasForeignKey(d => d.CheckInOutId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_VehicleCondition_CheckInOut");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
