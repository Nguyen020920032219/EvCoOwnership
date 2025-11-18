using System;
using System.Collections.Generic;

namespace ContractGroupService.Data.Entities;

public partial class VoteChoice
{
    public int CoOwnerChoiceId { get; set; }

    public int VoteId { get; set; }

    public int UserId { get; set; }

    public bool Choice { get; set; }

    public DateTime VotedAt { get; set; }

    public virtual GroupVote Vote { get; set; } = null!;
}
