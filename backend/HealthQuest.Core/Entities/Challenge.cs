namespace HealthQuest.Core.Entities;

public class Challenge
{
    public Guid Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public ChallengeType Type { get; set; }
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
    public int TotalPoints { get; set; }
    public int? MaxParticipants { get; set; }
    public bool IsActive { get; set; } = true;
    
    public Guid? CreatedByProviderId { get; set; }
    public DateTime CreatedAt { get; set; }
    
    // Navigation properties
    public Provider? CreatedByProvider { get; set; }
    public ICollection<ChallengeParticipant> Participants { get; set; } = new List<ChallengeParticipant>();
}

public enum ChallengeType
{
    DailySteps,
    WeeklyMedication,
    ExerciseStreak,
    HealthMetrics,
    Custom
}

public class ChallengeParticipant
{
    public Guid Id { get; set; }
    public Guid ChallengeId { get; set; }
    public Guid PatientId { get; set; }
    public int Progress { get; set; } = 0;
    public int PointsEarned { get; set; } = 0;
    public DateTime? CompletedAt { get; set; }
    public int Rank { get; set; } = 0;
    public DateTime JoinedAt { get; set; }
    
    // Navigation properties
    public Challenge Challenge { get; set; } = null!;
    public Patient Patient { get; set; } = null!;
}
