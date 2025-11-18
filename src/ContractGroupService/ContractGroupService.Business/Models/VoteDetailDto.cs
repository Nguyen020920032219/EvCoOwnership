namespace ContractGroupService.Business.Models;

public class VoteDetailDto
{
    public int VoteId { get; set; }
    public string Topic { get; set; } = string.Empty;
    public string? Description { get; set; }
    public string Status { get; set; } = string.Empty; // "Open" hoặc "Closed"
    public DateTime CreatedAt { get; set; }

    // Kết quả sơ bộ
    public int TotalYes { get; set; }
    public int TotalNo { get; set; }
    public List<VoteChoiceDto> Choices { get; set; } = new();
}