namespace HealthQuest.Core.Entities;

public class Patient
{
    public Guid Id { get; set; }
    public Guid UserId { get; set; }
    public string Name { get; set; } = string.Empty;
    public DateTime DateOfBirth { get; set; }
    public string Condition { get; set; } = string.Empty; // Diabetes, Heart Disease, etc.
    
    // Gamification
    public int Level { get; set; } = 1;
    public int TotalPoints { get; set; } = 0;
    public int CurrentStreak { get; set; } = 0;
    public DateTime? LastActivityDate { get; set; }
    public int LongestStreak { get; set; } = 0;
    
    // Compliance tracking
    public double ComplianceRate { get; set; } = 0;
    
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
    
    // Navigation properties
    public User User { get; set; } = null!;
    public ICollection<Quest> Quests { get; set; } = new List<Quest>();
    public ICollection<PatientAchievement> PatientAchievements { get; set; } = new List<PatientAchievement>();
    public ICollection<PointTransaction> PointTransactions { get; set; } = new List<PointTransaction>();
    public ICollection<HealthMetric> HealthMetrics { get; set; } = new List<HealthMetric>();
    public ICollection<ChallengeParticipant> ChallengeParticipations { get; set; } = new List<ChallengeParticipant>();
}
