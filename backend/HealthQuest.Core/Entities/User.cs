namespace HealthQuest.Core.Entities;

public class User
{
    public Guid Id { get; set; }
    public string Email { get; set; } = string.Empty;
    public string PasswordHash { get; set; } = string.Empty;
    public UserRole Role { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? LastLoginAt { get; set; }
    
    // Navigation properties
    public Patient? Patient { get; set; }
    public Provider? Provider { get; set; }
}

public enum UserRole
{
    Patient,
    Provider,
    Admin
}
