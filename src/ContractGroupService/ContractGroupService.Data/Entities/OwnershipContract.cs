using System;
using System.Collections.Generic;

namespace ContractGroupService.Data.Entities;

public partial class OwnershipContract
{
    public int ContractId { get; set; }

    public DateTime CreatedAt { get; set; }

    public DateTime UpdatedAt { get; set; }

    public string Content { get; set; } = null!;

    public string? Appendix { get; set; }

    public virtual CoOwnershipGroup? CoOwnershipGroup { get; set; }

    public virtual ICollection<ContractAppendix> ContractAppendices { get; set; } = new List<ContractAppendix>();

    public virtual ICollection<ContractSignature> ContractSignatures { get; set; } = new List<ContractSignature>();
}
