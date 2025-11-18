using System;
using System.Collections.Generic;

namespace ContractGroupService.Data.Entities;

public partial class GroupDisputeMessage
{
    public int GroupDisputeMessageId { get; set; }

    public int GroupDisputeId { get; set; }

    public int SenderUserId { get; set; }

    public string Message { get; set; } = null!;

    public DateTime CreatedAt { get; set; }

    public virtual GroupDispute GroupDispute { get; set; } = null!;
}
