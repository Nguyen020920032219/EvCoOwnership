namespace ContractGroupService.Data.Entities;

public class OwnershipShare
{
    public int CoOwnerGroupId { get; set; }

    public int UserId { get; set; }

    public decimal OwnershipPercent { get; set; }

    public virtual CoOwnershipGroup CoOwnerGroup { get; set; } = null!;
}