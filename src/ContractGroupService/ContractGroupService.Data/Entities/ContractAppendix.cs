namespace ContractGroupService.Data.Entities;

public class ContractAppendix
{
    public int AppendixId { get; set; }

    public int Status { get; set; }

    public string Content { get; set; } = null!;

    public DateTime CreatedAt { get; set; }

    public int ContractId { get; set; }

    public virtual OwnershipContract Contract { get; set; } = null!;
}