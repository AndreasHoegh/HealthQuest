using Microsoft.EntityFrameworkCore;
using HealthQuest.Core.Entities;
using HealthQuest.Infrastructure.Data;

namespace HealthQuest.API.Services;

public interface IQuestService
{
    Task<List<Quest>> GetDailyQuestsAsync(Guid patientId);
    Task<Quest?> CompleteQuestAsync(Guid questId, Guid patientId, string? verificationData = null);
    Task GenerateDailyQuestsAsync(Guid patientId);
}

public class QuestService : IQuestService
{
    private readonly HealthQuestDbContext _context;
    private readonly IPointsService _pointsService;

    public QuestService(HealthQuestDbContext context, IPointsService pointsService)
    {
        _context = context;
        _pointsService = pointsService;
    }

    public async Task<List<Quest>> GetDailyQuestsAsync(Guid patientId)
    {
        var today = DateTime.UtcNow.Date;
        var tomorrow = today.AddDays(1);

        return await _context.Quests
            .Where(q => q.PatientId == patientId && q.DueDate >= today && q.DueDate < tomorrow)
            .OrderBy(q => q.IsCompleted)
            .ThenBy(q => q.CreatedAt)
            .Select(q => new Quest
            {
                Id = q.Id,
                PatientId = q.PatientId,
                Type = q.Type,
                Title = q.Title,
                Description = q.Description,
                Points = q.Points,
                DueDate = q.DueDate,
                CompletedAt = q.CompletedAt,
                IsCompleted = q.IsCompleted,
                VerificationData = q.VerificationData,
                CreatedAt = q.CreatedAt
            })
            .ToListAsync();
    }

    public async Task<Quest?> CompleteQuestAsync(Guid questId, Guid patientId, string? verificationData = null)
    {
        var quest = await _context.Quests
            .FirstOrDefaultAsync(q => q.Id == questId && q.PatientId == patientId);

        if (quest == null || quest.IsCompleted)
            return null;

        quest.IsCompleted = true;
        quest.CompletedAt = DateTime.UtcNow;
        quest.VerificationData = verificationData;

        // Award points
        await _pointsService.AwardPointsAsync(
            patientId, 
            quest.Points, 
            TransactionType.QuestCompleted, 
            $"Completed: {quest.Title}",
            quest.Id.ToString()
        );

        await _context.SaveChangesAsync();

        // Update streak
        await UpdateStreakAsync(patientId);

        return quest;
    }

    public async Task GenerateDailyQuestsAsync(Guid patientId)
    {
        var patient = await _context.Patients.FindAsync(patientId);
        if (patient == null) return;

        var today = DateTime.UtcNow.Date;
        var tomorrow = today.AddDays(1);

        // Check if quests already exist for today
        var existingQuests = await _context.Quests
            .AnyAsync(q => q.PatientId == patientId && q.DueDate >= today && q.DueDate < tomorrow);

        if (existingQuests) return;

        // Generate standard quests based on patient condition
        var quests = new List<Quest>();

        // Medication quest
        quests.Add(new Quest
        {
            Id = Guid.NewGuid(),
            PatientId = patientId,
            Type = QuestType.Medication,
            Title = "Take Morning Medication",
            Description = "Remember to take your prescribed medication",
            Points = 50,
            DueDate = today.AddHours(23).AddMinutes(59),
            CreatedAt = DateTime.UtcNow
        });

        // Health measurement based on condition
        if (patient.Condition.Contains("Diabetes", StringComparison.OrdinalIgnoreCase))
        {
            quests.Add(new Quest
            {
                Id = Guid.NewGuid(),
                PatientId = patientId,
                Type = QuestType.HealthMeasurement,
                Title = "Measure Blood Glucose",
                Description = "Record your blood glucose level",
                Points = 30,
                DueDate = today.AddHours(23).AddMinutes(59),
                CreatedAt = DateTime.UtcNow
            });
        }

        // Daily steps
        quests.Add(new Quest
        {
            Id = Guid.NewGuid(),
            PatientId = patientId,
            Type = QuestType.DailySteps,
            Title = "Walk 5000 Steps",
            Description = "Stay active and reach your daily step goal",
            Points = 100,
            DueDate = today.AddHours(23).AddMinutes(59),
            CreatedAt = DateTime.UtcNow
        });

        _context.Quests.AddRange(quests);
        await _context.SaveChangesAsync();
    }

    private async Task UpdateStreakAsync(Guid patientId)
    {
        var patient = await _context.Patients.FindAsync(patientId);
        if (patient == null) return;

        var today = DateTime.UtcNow.Date;
        var yesterday = today.AddDays(-1);

        // Check if all quests for today are completed
        var todayQuests = await _context.Quests
            .Where(q => q.PatientId == patientId && q.DueDate >= today && q.DueDate < today.AddDays(1))
            .ToListAsync();

        if (todayQuests.Any() && todayQuests.All(q => q.IsCompleted))
        {
            // Check if patient completed quests yesterday
            if (patient.LastActivityDate?.Date == yesterday)
            {
                patient.CurrentStreak++;
            }
            else if (patient.LastActivityDate?.Date != today)
            {
                patient.CurrentStreak = 1;
            }

            patient.LastActivityDate = DateTime.UtcNow;

            if (patient.CurrentStreak > patient.LongestStreak)
            {
                patient.LongestStreak = patient.CurrentStreak;
            }

            // Award streak bonus
            if (patient.CurrentStreak % 7 == 0)
            {
                await _pointsService.AwardPointsAsync(
                    patientId,
                    100,
                    TransactionType.StreakBonus,
                    $"{patient.CurrentStreak} day streak bonus!"
                );
            }

            await _context.SaveChangesAsync();
        }
    }
}
