namespace ContractGroupService.Business.Models;

public class VoteChoiceDto
{
    public int UserId { get; set; }
    public string Choice { get; set; } = string.Empty; // "Yes" / "No"
    public DateTime VotedAt { get; set; }
}