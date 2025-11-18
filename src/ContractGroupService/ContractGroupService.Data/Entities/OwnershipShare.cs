using System;
using System.Collections.Generic;

namespace ContractGroupService.Data.Entities;

public partial class OwnershipShare
{
    public int CoOwnerGroupId { get; set; }

    public int UserId { get; set; }

    public decimal OwnershipPercent { get; set; }

    public virtual CoOwnershipGroup CoOwnerGroup { get; set; } = null!;
}
