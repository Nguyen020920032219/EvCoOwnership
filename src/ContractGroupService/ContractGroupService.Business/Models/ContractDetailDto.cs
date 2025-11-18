namespace ContractGroupService.Business.Models;

public class ContractDetailDto
{
    public int ContractId { get; set; }
    public int CoOwnerGroupId { get; set; }
    public string Content { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }
    
    // Danh sách trạng thái chữ ký
    public List<SignatureStatusDto> Signatures { get; set; } = new();
}