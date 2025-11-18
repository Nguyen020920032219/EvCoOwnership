namespace ContractGroupService.Business.Models;

public class CreateDisputeRequest
{
    public int CoOwnerGroupId { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public int? RelatedBookingId { get; set; }
}