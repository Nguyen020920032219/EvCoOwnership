namespace ContractGroupService.Business.Models;

public class GenerateContractRequest
{
    public int CoOwnerGroupId { get; set; }
    public string Content { get; set; } = string.Empty; // Nội dung điều khoản
}