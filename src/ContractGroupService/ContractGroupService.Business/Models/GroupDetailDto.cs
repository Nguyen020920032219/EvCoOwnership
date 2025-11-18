namespace ContractGroupService.Business.Models;

public class GroupDetailDto
{
    public int GroupId { get; set; }
    public string GroupName { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }
    public List<MemberShareDto> Members { get; set; } = new();
}