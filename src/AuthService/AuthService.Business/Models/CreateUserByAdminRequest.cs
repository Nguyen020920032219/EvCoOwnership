namespace AuthService.Business.Models;

public class CreateUserByAdminRequest
{
    public string PhoneNumber { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public int RoleId { get; set; } // 1=Admin, 2=Staff, 3=CoOwner
}