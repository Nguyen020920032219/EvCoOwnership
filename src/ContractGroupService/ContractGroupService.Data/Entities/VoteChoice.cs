namespace ContractGroupService.Data.Entities;

public class VoteChoice
{
    public int CoOwnerChoiceId { get; set; }

    public int VoteId { get; set; }

    public int UserId { get; set; }

    public bool Choice { get; set; }

    public DateTime VotedAt { get; set; }

    public virtual GroupVote Vote { get; set; } = null!;
}