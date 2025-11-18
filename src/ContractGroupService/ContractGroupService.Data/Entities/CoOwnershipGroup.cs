namespace ContractGroupService.Data.Entities;

public class CoOwnershipGroup
{
    public int CoOwnerGroupId { get; set; }

    public string GroupName { get; set; } = null!;

    public DateTime CreatedAt { get; set; }

    public DateTime UpdatedAt { get; set; }

    public int ContractId { get; set; }

    public virtual ICollection<CoOwnershipMember> CoOwnershipMembers { get; set; } = new List<CoOwnershipMember>();

    public virtual OwnershipContract Contract { get; set; } = null!;

    public virtual ICollection<GroupDispute> GroupDisputes { get; set; } = new List<GroupDispute>();

    public virtual ICollection<GroupVote> GroupVotes { get; set; } = new List<GroupVote>();

    public virtual ICollection<OwnershipShare> OwnershipShares { get; set; } = new List<OwnershipShare>();
}