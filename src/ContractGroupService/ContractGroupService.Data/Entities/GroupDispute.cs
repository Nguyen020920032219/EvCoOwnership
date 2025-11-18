namespace ContractGroupService.Data.Entities;

public class GroupDispute
{
    public int GroupDisputeId { get; set; }

    public int CoOwnershipGroupId { get; set; }

    public int RaisedByUserId { get; set; }

    public int? RelatedBookingId { get; set; }

    public string Title { get; set; } = null!;

    public string Description { get; set; } = null!;

    public int Status { get; set; }

    public DateTime CreatedAt { get; set; }

    public DateTime? ResolvedAt { get; set; }

    public int? ResolvedByStaffId { get; set; }

    public virtual CoOwnershipGroup CoOwnershipGroup { get; set; } = null!;

    public virtual ICollection<GroupDisputeMessage> GroupDisputeMessages { get; set; } =
        new List<GroupDisputeMessage>();
}