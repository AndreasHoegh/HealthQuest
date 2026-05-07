using Microsoft.EntityFrameworkCore;
using HealthQuest.Core.Entities;

namespace HealthQuest.Infrastructure.Data;

public class HealthQuestDbContext : DbContext
{
    public HealthQuestDbContext(DbContextOptions<HealthQuestDbContext> options) : base(options)
    {
    }

    public DbSet<User> Users { get; set; }
    public DbSet<Patient> Patients { get; set; }
    public DbSet<Provider> Providers { get; set; }
    public DbSet<Quest> Quests { get; set; }
    public DbSet<Challenge> Challenges { get; set; }
    public DbSet<ChallengeParticipant> ChallengeParticipants { get; set; }
    public DbSet<Achievement> Achievements { get; set; }
    public DbSet<PatientAchievement> PatientAchievements { get; set; }
    public DbSet<PointTransaction> PointTransactions { get; set; }
    public DbSet<HealthMetric> HealthMetrics { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // User configuration
        modelBuilder.Entity<User>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.HasIndex(e => e.Email).IsUnique();
            entity.Property(e => e.Email).IsRequired().HasMaxLength(255);
            entity.Property(e => e.PasswordHash).IsRequired();
            
            entity.HasOne(e => e.Patient)
                .WithOne(p => p.User)
                .HasForeignKey<Patient>(p => p.UserId)
                .OnDelete(DeleteBehavior.Cascade);
                
            entity.HasOne(e => e.Provider)
                .WithOne(p => p.User)
                .HasForeignKey<Provider>(p => p.UserId)
                .OnDelete(DeleteBehavior.Cascade);
        });

        // Patient configuration
        modelBuilder.Entity<Patient>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Name).IsRequired().HasMaxLength(200);
            entity.Property(e => e.Condition).HasMaxLength(100);
            
            entity.HasMany(e => e.Quests)
                .WithOne(q => q.Patient)
                .HasForeignKey(q => q.PatientId)
                .OnDelete(DeleteBehavior.Cascade);
                
            entity.HasMany(e => e.HealthMetrics)
                .WithOne(h => h.Patient)
                .HasForeignKey(h => h.PatientId)
                .OnDelete(DeleteBehavior.Cascade);
        });

        // Provider configuration
        modelBuilder.Entity<Provider>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Name).IsRequired().HasMaxLength(200);
            entity.Property(e => e.Specialty).HasMaxLength(100);
            entity.Property(e => e.HospitalName).HasMaxLength(200);
        });

        // Quest configuration
        modelBuilder.Entity<Quest>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Title).IsRequired().HasMaxLength(200);
            entity.Property(e => e.Description).HasMaxLength(1000);
            
            entity.HasIndex(e => new { e.PatientId, e.DueDate });
            entity.HasIndex(e => e.IsCompleted);
        });

        // Challenge configuration
        modelBuilder.Entity<Challenge>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Title).IsRequired().HasMaxLength(200);
            entity.Property(e => e.Description).HasMaxLength(1000);
            
            entity.HasIndex(e => e.IsActive);
            entity.HasIndex(e => new { e.StartDate, e.EndDate });
        });

        // ChallengeParticipant configuration
        modelBuilder.Entity<ChallengeParticipant>(entity =>
        {
            entity.HasKey(e => e.Id);
            
            entity.HasIndex(e => new { e.ChallengeId, e.Rank });
            entity.HasIndex(e => new { e.ChallengeId, e.PatientId }).IsUnique();
            
            entity.HasOne(e => e.Challenge)
                .WithMany(c => c.Participants)
                .HasForeignKey(e => e.ChallengeId)
                .OnDelete(DeleteBehavior.Cascade);
                
            entity.HasOne(e => e.Patient)
                .WithMany(p => p.ChallengeParticipations)
                .HasForeignKey(e => e.PatientId)
                .OnDelete(DeleteBehavior.Cascade);
        });

        // Achievement configuration
        modelBuilder.Entity<Achievement>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Name).IsRequired().HasMaxLength(100);
            entity.Property(e => e.Description).HasMaxLength(500);
            entity.Property(e => e.Icon).HasMaxLength(50);
        });

        // PatientAchievement configuration
        modelBuilder.Entity<PatientAchievement>(entity =>
        {
            entity.HasKey(e => e.Id);
            
            entity.HasIndex(e => new { e.PatientId, e.AchievementId }).IsUnique();
            
            entity.HasOne(e => e.Patient)
                .WithMany(p => p.PatientAchievements)
                .HasForeignKey(e => e.PatientId)
                .OnDelete(DeleteBehavior.Cascade);
                
            entity.HasOne(e => e.Achievement)
                .WithMany(a => a.PatientAchievements)
                .HasForeignKey(e => e.AchievementId)
                .OnDelete(DeleteBehavior.Cascade);
        });

        // PointTransaction configuration
        modelBuilder.Entity<PointTransaction>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Description).HasMaxLength(500);
            
            entity.HasIndex(e => new { e.PatientId, e.CreatedAt });
            
            entity.HasOne(e => e.Patient)
                .WithMany(p => p.PointTransactions)
                .HasForeignKey(e => e.PatientId)
                .OnDelete(DeleteBehavior.Cascade);
        });

        // HealthMetric configuration
        modelBuilder.Entity<HealthMetric>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Unit).HasMaxLength(20);
            entity.Property(e => e.Notes).HasMaxLength(500);
            
            entity.HasIndex(e => new { e.PatientId, e.Type, e.RecordedAt });
        });
    }
}
