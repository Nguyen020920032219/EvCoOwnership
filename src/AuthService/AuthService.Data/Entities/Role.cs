namespace AuthService.Data.Entities;

public class Role
{
    public int RoleId { get; set; }

    public string RoleName { get; set; } = null!;

    public virtual ICollection<AppUser> AppUsers { get; set; } = new List<AppUser>();
}