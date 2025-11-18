using System;
using System.Collections.Generic;

namespace ContractGroupService.Data.Entities;

public partial class CoOwnershipMember
{
    public int UserId { get; set; }

    public int CoOwnerGroupId { get; set; }

    public bool IsGroupAdmin { get; set; }

    public virtual CoOwnershipGroup CoOwnerGroup { get; set; } = null!;
}
