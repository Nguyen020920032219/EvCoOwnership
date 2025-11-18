using FinanceService.Data.Entities;
using Microsoft.EntityFrameworkCore;

namespace FinanceService.Data.Configurations;

public partial class FinanceDbContext : DbContext
{
    public FinanceDbContext()
    {
    }

    public FinanceDbContext(DbContextOptions<FinanceDbContext> options)
        : base(options)
    {
    }

    public virtual DbSet<GroupFund> GroupFunds { get; set; }

    public virtual DbSet<GroupFundHistory> GroupFundHistories { get; set; }

    public virtual DbSet<PaymentTransaction> PaymentTransactions { get; set; }

    public virtual DbSet<SharedExpense> SharedExpenses { get; set; }

    public virtual DbSet<SharedExpenseShare> SharedExpenseShares { get; set; }

    public virtual DbSet<VehicleMaintenanceDetail> VehicleMaintenanceDetails { get; set; }

    public virtual DbSet<VehicleMaintenanceInvoice> VehicleMaintenanceInvoices { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<GroupFund>(entity =>
        {
            entity.HasKey(e => e.FundId).HasName("PK__GroupFun__830DFC5AF61B0976");

            entity.ToTable("GroupFund");

            entity.Property(e => e.Amount).HasColumnType("decimal(18, 2)");
        });

        modelBuilder.Entity<GroupFundHistory>(entity =>
        {
            entity.HasKey(e => e.FundHistoryId).HasName("PK__GroupFun__641AC35E7D42A542");

            entity.ToTable("GroupFundHistory");

            entity.Property(e => e.ChangeAmount).HasColumnType("decimal(18, 2)");
            entity.Property(e => e.ChangedAt).HasDefaultValueSql("(sysutcdatetime())");
            entity.Property(e => e.Reason).HasMaxLength(255);

            entity.HasOne(d => d.Fund).WithMany(p => p.GroupFundHistories)
                .HasForeignKey(d => d.FundId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_FundHistory_Fund");
        });

        modelBuilder.Entity<PaymentTransaction>(entity =>
        {
            entity.HasKey(e => e.PaymentTransactionId).HasName("PK__PaymentT__C22AEFE0E258E4A7");

            entity.ToTable("PaymentTransaction");

            entity.HasIndex(e => new { e.UserId, e.CreatedAt }, "IX_PaymentTransaction_UserDate");

            entity.Property(e => e.Amount).HasColumnType("decimal(18, 2)");
            entity.Property(e => e.CreatedAt).HasDefaultValueSql("(sysutcdatetime())");
            entity.Property(e => e.PaymentMethod).HasMaxLength(50);
            entity.Property(e => e.ProviderTransactionId).HasMaxLength(100);

            entity.HasOne(d => d.SharedExpense).WithMany(p => p.PaymentTransactions)
                .HasForeignKey(d => d.SharedExpenseId)
                .HasConstraintName("FK_PaymentTransaction_SharedExpense");
        });

        modelBuilder.Entity<SharedExpense>(entity =>
        {
            entity.HasKey(e => e.ExpenseId).HasName("PK__SharedEx__1445CFD3BE4DAE6E");

            entity.ToTable("SharedExpense");

            entity.Property(e => e.Amount).HasColumnType("decimal(18, 2)");
            entity.Property(e => e.ExpenseType).HasMaxLength(50);
            entity.Property(e => e.ShareMode).HasMaxLength(50);
        });

        modelBuilder.Entity<SharedExpenseShare>(entity =>
        {
            entity.HasKey(e => e.ExpenseShareId).HasName("PK__SharedEx__373CB010B88CBA3F");

            entity.ToTable("SharedExpenseShare");

            entity.Property(e => e.ShareAmount).HasColumnType("decimal(18, 2)");

            entity.HasOne(d => d.Expense).WithMany(p => p.SharedExpenseShares)
                .HasForeignKey(d => d.ExpenseId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_ExpenseShare_Expense");
        });

        modelBuilder.Entity<VehicleMaintenanceDetail>(entity =>
        {
            entity.HasKey(e => e.MaintenanceDetailId).HasName("PK__VehicleM__1D992F54D4CD958A");

            entity.ToTable("VehicleMaintenanceDetail");

            entity.Property(e => e.Price).HasColumnType("decimal(18, 2)");
            entity.Property(e => e.Service).HasMaxLength(255);

            entity.HasOne(d => d.MaintenanceInvoice).WithMany(p => p.VehicleMaintenanceDetails)
                .HasForeignKey(d => d.MaintenanceInvoiceId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_MaintenanceDetail_Invoice");
        });

        modelBuilder.Entity<VehicleMaintenanceInvoice>(entity =>
        {
            entity.HasKey(e => e.MaintenanceInvoiceId).HasName("PK__VehicleM__DA090CAD893FAF40");

            entity.ToTable("VehicleMaintenanceInvoice");

            entity.Property(e => e.Amount).HasColumnType("decimal(18, 2)");
            entity.Property(e => e.Description).HasMaxLength(255);

            entity.HasOne(d => d.Fund).WithMany(p => p.VehicleMaintenanceInvoices)
                .HasForeignKey(d => d.FundId)
                .HasConstraintName("FK_MaintenanceInvoice_Fund");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}