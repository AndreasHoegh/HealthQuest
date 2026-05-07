using Microsoft.EntityFrameworkCore;
using HealthQuest.Core.Entities;
using HealthQuest.Infrastructure.Data;

namespace HealthQuest.API.Services;

public interface IPointsService
{
    Task AwardPointsAsync(Guid patientId, int points, TransactionType type, string description, string? referenceId = null);
    Task<int> GetPointsBalanceAsync(Guid patientId);
    Task<List<PointTransaction>> GetTransactionHistoryAsync(Guid patientId, int limit = 50);
}

public class PointsService : IPointsService
{
    private readonly HealthQuestDbContext _context;
    private const int PointsPerLevel = 1000;

    public PointsService(HealthQuestDbContext context)
    {
        _context = context;
    }

    public async Task AwardPointsAsync(Guid patientId, int points, TransactionType type, string description, string? referenceId = null)
    {
        var patient = await _context.Patients.FindAsync(patientId);
        if (patient == null) return;

        // Create transaction
        var transaction = new PointTransaction
        {
            Id = Guid.NewGuid(),
            PatientId = patientId,
            Amount = points,
            Type = type,
            Description = description,
            ReferenceId = referenceId,
            CreatedAt = DateTime.UtcNow
        };

        _context.PointTransactions.Add(transaction);

        // Update patient points
        var oldPoints = patient.TotalPoints;
        patient.TotalPoints += points;
        patient.UpdatedAt = DateTime.UtcNow;

        // Check for level up
        var oldLevel = patient.Level;
        var newLevel = CalculateLevel(patient.TotalPoints);

        if (newLevel > oldLevel)
        {
            patient.Level = newLevel;

            // Award level up bonus
            var levelBonus = new PointTransaction
            {
                Id = Guid.NewGuid(),
                PatientId = patientId,
                Amount = newLevel * 50,
                Type = TransactionType.LevelUp,
                Description = $"Level {newLevel} reached! Bonus points!",
                CreatedAt = DateTime.UtcNow
            };

            _context.PointTransactions.Add(levelBonus);
            patient.TotalPoints += levelBonus.Amount;
        }

        // Update compliance rate
        await UpdateComplianceRateAsync(patient);

        await _context.SaveChangesAsync();
    }

    public async Task<int> GetPointsBalanceAsync(Guid patientId)
    {
        var patient = await _context.Patients.FindAsync(patientId);
        return patient?.TotalPoints ?? 0;
    }

    public async Task<List<PointTransaction>> GetTransactionHistoryAsync(Guid patientId, int limit = 50)
    {
        return await _context.PointTransactions
            .Where(t => t.PatientId == patientId)
            .OrderByDescending(t => t.CreatedAt)
            .Take(limit)
            .ToListAsync();
    }

    private int CalculateLevel(int totalPoints)
    {
        return (totalPoints / PointsPerLevel) + 1;
    }

    private async Task UpdateComplianceRateAsync(Patient patient)
    {
        var thirtyDaysAgo = DateTime.UtcNow.AddDays(-30);

        var recentQuests = await _context.Quests
            .Where(q => q.PatientId == patient.Id && q.CreatedAt >= thirtyDaysAgo)
            .ToListAsync();

        if (recentQuests.Any())
        {
            var completedCount = recentQuests.Count(q => q.IsCompleted);
            patient.ComplianceRate = Math.Round((double)completedCount / recentQuests.Count * 100, 2);
        }
    }
}
