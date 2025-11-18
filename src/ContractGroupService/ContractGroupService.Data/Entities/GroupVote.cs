namespace ContractGroupService.Data.Entities;

public class GroupVote
{
    public int VoteId { get; set; }

    public int CoOwnerGroupId { get; set; }

    public int UserId { get; set; }

    public string Topic { get; set; } = null!;

    public string? Description { get; set; }

    public int Status { get; set; }

    public DateTime CreatedAt { get; set; }

    public virtual CoOwnershipGroup CoOwnerGroup { get; set; } = null!;

    public virtual ICollection<VoteChoice> VoteChoices { get; set; } = new List<VoteChoice>();
}