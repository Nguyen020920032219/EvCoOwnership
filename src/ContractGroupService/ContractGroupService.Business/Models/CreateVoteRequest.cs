namespace ContractGroupService.Business.Models;

public class CreateVoteRequest
{
    public int CoOwnerGroupId { get; set; }
    public string Topic { get; set; } = string.Empty;
    public string? Description { get; set; }
}