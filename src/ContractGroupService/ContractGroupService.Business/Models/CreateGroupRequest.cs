namespace ContractGroupService.Business.Models;

public class CreateGroupRequest
{
    public string GroupName { get; set; } = string.Empty;

    // Danh sách thành viên ban đầu (ngoài người tạo)
    public List<MemberShareDto> InitialMembers { get; set; } = new();
}