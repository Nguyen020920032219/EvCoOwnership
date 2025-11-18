namespace AuthService.Data.Entities;

public class UserProfile
{
    public int ProfileId { get; set; }

    public string? Email { get; set; }

    public string FirstName { get; set; } = null!;

    public string LastName { get; set; } = null!;

    public string? Gender { get; set; }

    public DateOnly? DateOfBirth { get; set; }

    public string? Address { get; set; }

    public int UserId { get; set; }

    public virtual AppUser User { get; set; } = null!;
}