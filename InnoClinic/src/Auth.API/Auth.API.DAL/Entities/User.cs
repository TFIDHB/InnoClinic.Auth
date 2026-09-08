namespace InnoClinic.Auth.API.DAL.Entities;

public class User
{
    public required Guid Id { get; set; }

    public required string Email { get; set; }

    public required string PasswordHash { get; set; }

    public string? RefreshToken { get; set; }

    public DateTime? RefreshTokenExpiry { get; set; }

    public string? PhoneNumber { get; set; }

    public bool IsEmailVerified { get; set; } = false;

    public Guid? PhotoId { get; set; }

    public DateTime CreatedAt { get; set; }

    public DateTime? UpdatedAt { get; set; }
}
