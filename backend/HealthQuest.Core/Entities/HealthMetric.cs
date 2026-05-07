namespace HealthQuest.Core.Entities;

public class HealthMetric
{
    public Guid Id { get; set; }
    public Guid PatientId { get; set; }
    public MetricType Type { get; set; }
    public double Value { get; set; }
    public string Unit { get; set; } = string.Empty;
    public DateTime RecordedAt { get; set; }
    public string? Notes { get; set; }
    
    // Navigation properties
    public Patient Patient { get; set; } = null!;
}

public enum MetricType
{
    BloodGlucose,      // mmol/L or mg/dL
    BloodPressure,     // Stored as systolic value, diastolic in notes
    Weight,            // kg
    HeartRate,         // bpm
    OxygenLevel,       // %
    Temperature,       // Celsius
    PainLevel,         // 1-10 scale
    MoodScore,         // 1-10 scale
    Steps,             // count
    Sleep,             // hours
    WaterIntake        // liters
}
