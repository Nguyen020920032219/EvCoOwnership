namespace ContractGroupService.Business.Models;

public class MemberShareDto
{
    public int UserId { get; set; }
    public string UserName { get; set; } = string.Empty;
    public string PhoneNumber { get; set; } = string.Empty;
    public decimal SharePercent { get; set; }
    public bool IsAdmin { get; set; } = false;
}