namespace ContractGroupService.Business.Models;

public class DisputeMessageDto
{
    public int SenderUserId { get; set; }
    public string Content { get; set; } = string.Empty;
    public DateTime SentAt { get; set; }
}