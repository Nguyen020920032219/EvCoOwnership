namespace ContractGroupService.Data.Entities;

public class CoOwnershipMember
{
    public int UserId { get; set; }

    public int CoOwnerGroupId { get; set; }

    public bool IsGroupAdmin { get; set; }

    public virtual CoOwnershipGroup CoOwnerGroup { get; set; } = null!;
}