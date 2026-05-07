namespace HealthQuest.Core.Entities;

public class Quest
{
    public Guid Id { get; set; }
    public Guid PatientId { get; set; }
    public QuestType Type { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public int Points { get; set; }
    public DateTime DueDate { get; set; }
    public DateTime? CompletedAt { get; set; }
    public bool IsCompleted { get; set; }
    
    // Verification
    public string? VerificationData { get; set; } // JSON for images, values, etc.
    
    public DateTime CreatedAt { get; set; }
    
    // Navigation properties
    public Patient Patient { get; set; } = null!;
}

public enum QuestType
{
    Medication,
    Exercise,
    HealthMeasurement,
    DailySteps,
    WaterIntake,
    Sleep,
    Custom
}
