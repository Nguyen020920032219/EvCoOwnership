namespace ContractGroupService.Business.Models;

public class CoOwnerGroupDto
{
    public int CoOwnerGroupId { get; set; }
    public string GroupName { get; set; } = string.Empty;
    public int ContractId { get; set; }
    public DateTime CreatedAt { get; set; }
}