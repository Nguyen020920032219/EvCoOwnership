using System;
using System.Collections.Generic;

namespace ContractGroupService.Data.Entities;

public partial class ContractSignature
{
    public int ContractSignatureId { get; set; }

    public int UserId { get; set; }

    public DateTime? SignedAt { get; set; }

    public int Status { get; set; }

    public int ContractId { get; set; }

    public virtual OwnershipContract Contract { get; set; } = null!;
}
