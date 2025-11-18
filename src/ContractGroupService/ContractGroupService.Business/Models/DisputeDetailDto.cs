namespace ContractGroupService.Business.Models;

public class DisputeDetailDto
{
    public int DisputeId { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Status { get; set; } = string.Empty; // Open, Resolved...
    public DateTime CreatedAt { get; set; }
    public int RaisedByUserId { get; set; }
    public List<DisputeMessageDto> Messages { get; set; } = new();
}