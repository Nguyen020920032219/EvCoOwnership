using ContractGroupService.Data.Entities;
using Microsoft.EntityFrameworkCore;

namespace ContractGroupService.Data.Configurations;

public partial class ContractGroupDbContext : DbContext
{
    public ContractGroupDbContext()
    {
    }

    public ContractGroupDbContext(DbContextOptions<ContractGroupDbContext> options)
        : base(options)
    {
    }

    public virtual DbSet<CoOwnershipGroup> CoOwnershipGroups { get; set; }

    public virtual DbSet<CoOwnershipMember> CoOwnershipMembers { get; set; }

    public virtual DbSet<ContractAppendix> ContractAppendices { get; set; }

    public virtual DbSet<ContractSignature> ContractSignatures { get; set; }

    public virtual DbSet<GroupDispute> GroupDisputes { get; set; }

    public virtual DbSet<GroupDisputeMessage> GroupDisputeMessages { get; set; }

    public virtual DbSet<GroupVote> GroupVotes { get; set; }

    public virtual DbSet<OwnershipContract> OwnershipContracts { get; set; }

    public virtual DbSet<OwnershipShare> OwnershipShares { get; set; }

    public virtual DbSet<VoteChoice> VoteChoices { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        optionsBuilder.UseSqlServer("Name=ConnectionStrings:ContractGroupDb");
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<CoOwnershipGroup>(entity =>
        {
            entity.HasKey(e => e.CoOwnerGroupId).HasName("PK__CoOwners__A0D4A9678DC8C63C");

            entity.ToTable("CoOwnershipGroup");

            entity.HasIndex(e => e.ContractId, "UQ__CoOwners__C90D3468E7B48BEA").IsUnique();

            entity.Property(e => e.CreatedAt).HasDefaultValueSql("(sysutcdatetime())");
            entity.Property(e => e.GroupName).HasMaxLength(255);
            entity.Property(e => e.UpdatedAt).HasDefaultValueSql("(sysutcdatetime())");

            entity.HasOne(d => d.Contract).WithOne(p => p.CoOwnershipGroup)
                .HasForeignKey<CoOwnershipGroup>(d => d.ContractId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_CoOwnerGroup_Contract");
        });

        modelBuilder.Entity<CoOwnershipMember>(entity =>
        {
            entity.HasKey(e => new { e.UserId, e.CoOwnerGroupId }).HasName("PK_CoOwnerParticipant");

            entity.ToTable("CoOwnershipMember");

            entity.HasOne(d => d.CoOwnerGroup).WithMany(p => p.CoOwnershipMembers)
                .HasForeignKey(d => d.CoOwnerGroupId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_CoOwnerParticipant_Group");
        });

        modelBuilder.Entity<ContractAppendix>(entity =>
        {
            entity.HasKey(e => e.AppendixId).HasName("PK__Contract__44B149C4F06C2FAC");

            entity.ToTable("ContractAppendix");

            entity.Property(e => e.CreatedAt).HasDefaultValueSql("(sysutcdatetime())");

            entity.HasOne(d => d.Contract).WithMany(p => p.ContractAppendices)
                .HasForeignKey(d => d.ContractId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Appendix_Contract");
        });

        modelBuilder.Entity<ContractSignature>(entity =>
        {
            entity.HasKey(e => e.ContractSignatureId).HasName("PK__Contract__6A8CC220EF584306");

            entity.ToTable("ContractSignature");

            entity.HasIndex(e => new { e.ContractId, e.UserId }, "UX_ContractSignatures").IsUnique();

            entity.HasOne(d => d.Contract).WithMany(p => p.ContractSignatures)
                .HasForeignKey(d => d.ContractId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_ContractSignatures_Contract");
        });

        modelBuilder.Entity<GroupDispute>(entity =>
        {
            entity.HasKey(e => e.GroupDisputeId).HasName("PK__GroupDis__EED902615ACD646C");

            entity.ToTable("GroupDispute");

            entity.Property(e => e.CreatedAt).HasDefaultValueSql("(sysutcdatetime())");
            entity.Property(e => e.Title).HasMaxLength(255);

            entity.HasOne(d => d.CoOwnershipGroup).WithMany(p => p.GroupDisputes)
                .HasForeignKey(d => d.CoOwnershipGroupId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_GroupDispute_Group");
        });

        modelBuilder.Entity<GroupDisputeMessage>(entity =>
        {
            entity.HasKey(e => e.GroupDisputeMessageId).HasName("PK__GroupDis__0D40F76F9A88B43E");

            entity.ToTable("GroupDisputeMessage");

            entity.Property(e => e.CreatedAt).HasDefaultValueSql("(sysutcdatetime())");

            entity.HasOne(d => d.GroupDispute).WithMany(p => p.GroupDisputeMessages)
                .HasForeignKey(d => d.GroupDisputeId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_GroupDisputeMessage_Dispute");
        });

        modelBuilder.Entity<GroupVote>(entity =>
        {
            entity.HasKey(e => e.VoteId).HasName("PK__GroupVot__52F015C2B5F5F8CD");

            entity.ToTable("GroupVote");

            entity.Property(e => e.CreatedAt).HasDefaultValueSql("(sysutcdatetime())");
            entity.Property(e => e.Topic).HasMaxLength(255);

            entity.HasOne(d => d.CoOwnerGroup).WithMany(p => p.GroupVotes)
                .HasForeignKey(d => d.CoOwnerGroupId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Vote_Group");
        });

        modelBuilder.Entity<OwnershipContract>(entity =>
        {
            entity.HasKey(e => e.ContractId).HasName("PK__Ownershi__C90D34693AD93CBE");

            entity.ToTable("OwnershipContract");

            entity.Property(e => e.CreatedAt).HasDefaultValueSql("(sysutcdatetime())");
            entity.Property(e => e.UpdatedAt).HasDefaultValueSql("(sysutcdatetime())");
        });

        modelBuilder.Entity<OwnershipShare>(entity =>
        {
            entity.HasKey(e => new { e.CoOwnerGroupId, e.UserId }).HasName("PK_OwnershipAllocation");

            entity.ToTable("OwnershipShare", tb => tb.HasTrigger("TR_OwnershipAllocation_Sum"));

            entity.Property(e => e.OwnershipPercent).HasColumnType("decimal(5, 2)");

            entity.HasOne(d => d.CoOwnerGroup).WithMany(p => p.OwnershipShares)
                .HasForeignKey(d => d.CoOwnerGroupId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_OwnershipAllocation_Group");
        });

        modelBuilder.Entity<VoteChoice>(entity =>
        {
            entity.HasKey(e => e.CoOwnerChoiceId).HasName("PK__VoteChoi__00DD39137652ABD1");

            entity.ToTable("VoteChoice");

            entity.HasIndex(e => new { e.VoteId, e.UserId }, "UX_CoOwnerChoice").IsUnique();

            entity.Property(e => e.VotedAt).HasDefaultValueSql("(sysutcdatetime())");

            entity.HasOne(d => d.Vote).WithMany(p => p.VoteChoices)
                .HasForeignKey(d => d.VoteId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_CoOwnerChoice_Vote");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}