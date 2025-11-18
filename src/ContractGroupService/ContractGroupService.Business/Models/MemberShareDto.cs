namespace ContractGroupService.Business.Models;

public class MemberShareDto
{
    public int UserId { get; set; }
    public decimal SharePercent { get; set; } // 0.3 = 30%
    public bool IsAdmin { get; set; } = false;
}