namespace HealthQuest.Core.Entities;

public class PointTransaction
{
    public Guid Id { get; set; }
    public Guid PatientId { get; set; }
    public int Amount { get; set; } // Can be positive or negative
    public TransactionType Type { get; set; }
    public string Description { get; set; } = string.Empty;
    public string? ReferenceId { get; set; } // Quest/Challenge/Achievement ID
    public DateTime CreatedAt { get; set; }
    
    // Navigation properties
    public Patient Patient { get; set; } = null!;
}

public enum TransactionType
{
    QuestCompleted,
    ChallengeCompleted,
    AchievementUnlocked,
    StreakBonus,
    LevelUp,
    RewardRedeemed,
    AdminAdjustment
}
