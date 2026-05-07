namespace HealthQuest.Core.Entities;

public class Provider
{
    public Guid Id { get; set; }
    public Guid UserId { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Specialty { get; set; } = string.Empty; // Cardiologist, Endocrinologist, etc.
    public string HospitalName { get; set; } = string.Empty;
    
    public DateTime CreatedAt { get; set; }
    
    // Navigation properties
    public User User { get; set; } = null!;
    public ICollection<Challenge> CreatedChallenges { get; set; } = new List<Challenge>();
}
