namespace HealthQuest.Core.Entities;

public class Achievement
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string Icon { get; set; } = string.Empty; // Emoji or icon name
    public AchievementCategory Category { get; set; }
    public int Points { get; set; }
    
    // Criteria (stored as JSON)
    public string CriteriaJson { get; set; } = string.Empty;
    
    public DateTime CreatedAt { get; set; }
    
    // Navigation properties
    public ICollection<PatientAchievement> PatientAchievements { get; set; } = new List<PatientAchievement>();
}

public enum AchievementCategory
{
    Consistency,
    Medication,
    Exercise,
    DataTracking,
    Social,
    Milestone
}

public class PatientAchievement
{
    public Guid Id { get; set; }
    public Guid PatientId { get; set; }
    public Guid AchievementId { get; set; }
    public DateTime UnlockedAt { get; set; }
    
    // Navigation properties
    public Patient Patient { get; set; } = null!;
    public Achievement Achievement { get; set; } = null!;
}
