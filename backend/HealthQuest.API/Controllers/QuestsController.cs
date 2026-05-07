using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using HealthQuest.API.Services;
using HealthQuest.Core.Entities;
using HealthQuest.Infrastructure.Data;

namespace HealthQuest.API.Controllers;

[Authorize]
[ApiController]
[Route("api/[controller]")]
public class QuestsController : ControllerBase
{
    private readonly IQuestService _questService;
    private readonly HealthQuestDbContext _context;

    public QuestsController(IQuestService questService, HealthQuestDbContext context)
    {
        _questService = questService;
        _context = context;
    }

    [HttpGet("daily")]
    public async Task<ActionResult<List<Quest>>> GetDailyQuests()
    {
        try
        {
            var patientId = await GetPatientIdFromClaimsAsync();
            if (patientId == null)
            {
                return BadRequest(new { message = "Patient not found for this user" });
            }

            var quests = await _questService.GetDailyQuestsAsync(patientId.Value);
            return Ok(quests);
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { 
                message = "Error retrieving quests", 
                error = ex.Message,
                innerError = ex.InnerException?.Message 
            });
        }
    }

    [HttpPost("{questId}/complete")]
    public async Task<ActionResult> CompleteQuest(Guid questId, [FromBody] CompleteQuestRequest? request = null)
    {
        try
        {
            var patientId = await GetPatientIdFromClaimsAsync();
            if (patientId == null)
                return BadRequest(new { message = "Patient not found" });

            var quest = await _questService.CompleteQuestAsync(questId, patientId.Value, request?.VerificationData);

            if (quest == null)
                return NotFound(new { message = "Quest not found or already completed" });

            // Return simplified DTO to avoid circular reference
            var questDto = new
            {
                quest.Id,
                quest.PatientId,
                quest.Type,
                quest.Title,
                quest.Description,
                quest.Points,
                quest.DueDate,
                quest.CompletedAt,
                quest.IsCompleted,
                quest.VerificationData,
                quest.CreatedAt
            };

            return Ok(questDto);
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { 
                message = "Error completing quest", 
                error = ex.Message 
            });
        }
    }

    [HttpPost("generate")]
    public async Task<ActionResult> GenerateDailyQuests()
    {
        try
        {
            var patientId = await GetPatientIdFromClaimsAsync();
            if (patientId == null)
            {
                return BadRequest(new { message = "Patient not found for this user" });
            }

            await _questService.GenerateDailyQuestsAsync(patientId.Value);
            return Ok(new { message = "Daily quests generated successfully" });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { 
                message = "Error generating quests", 
                error = ex.Message,
                innerError = ex.InnerException?.Message 
            });
        }
    }

    private async Task<Guid?> GetPatientIdFromClaimsAsync()
    {
        var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (!Guid.TryParse(userIdClaim, out var userId))
            return null;

        // Get patient from user ID
        var patient = await _context.Patients.FirstOrDefaultAsync(p => p.UserId == userId);
        return patient?.Id;
    }
}

public record CompleteQuestRequest(string? VerificationData = null);
