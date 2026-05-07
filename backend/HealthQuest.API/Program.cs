using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using HealthQuest.API.Services;
using HealthQuest.Core.Entities;
using HealthQuest.Infrastructure.Data;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();

// Swagger with JWT support
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "HealthQuest API", Version = "v1" });
    
    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Description = "JWT Authorization header using the Bearer scheme. Enter 'Bearer' [space] and then your token",
        Name = "Authorization",
        In = ParameterLocation.Header,
        Type = SecuritySchemeType.ApiKey,
        Scheme = "Bearer"
    });

    c.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            },
            Array.Empty<string>()
        }
    });
});

// IN-MEMORY Database (No PostgreSQL needed!)
builder.Services.AddDbContext<HealthQuestDbContext>(options =>
    options.UseInMemoryDatabase("HealthQuestDb"));

// JWT Authentication
var jwtSecret = builder.Configuration["JwtSettings:Secret"] 
    ?? throw new InvalidOperationException("JWT Secret not configured");

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = builder.Configuration["JwtSettings:Issuer"],
            ValidAudience = builder.Configuration["JwtSettings:Audience"],
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSecret))
        };
    });

builder.Services.AddAuthorization();

// CORS
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAngular", policy =>
    {
        policy.WithOrigins("http://localhost:4200", "http://localhost:4201")
              .AllowAnyHeader()
              .AllowAnyMethod()
              .AllowCredentials();
    });
});

// Application Services
builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddScoped<IQuestService, QuestService>();
builder.Services.AddScoped<IPointsService, PointsService>();

var app = builder.Build();

// Seed test data
using (var scope = app.Services.CreateScope())
{
    var context = scope.ServiceProvider.GetRequiredService<HealthQuestDbContext>();
    var authService = scope.ServiceProvider.GetRequiredService<IAuthService>();
    
    // Create test patient
    var testUser = new User
    {
        Id = Guid.NewGuid(),
        Email = "test@healthquest.dk",
        PasswordHash = authService.HashPassword("Test123!"),
        Role = UserRole.Patient,
        CreatedAt = DateTime.UtcNow
    };
    context.Users.Add(testUser);
    
    var testPatient = new Patient
    {
        Id = Guid.NewGuid(),
        UserId = testUser.Id,
        Name = "Test Patient",
        DateOfBirth = DateTime.UtcNow.AddYears(-35),
        Condition = "Diabetes",
        Level = 1,
        TotalPoints = 0,
        CurrentStreak = 0,
        CreatedAt = DateTime.UtcNow,
        UpdatedAt = DateTime.UtcNow
    };
    context.Patients.Add(testPatient);
    
    // Create test provider
    var providerUser = new User
    {
        Id = Guid.NewGuid(),
        Email = "doctor@healthquest.dk",
        PasswordHash = authService.HashPassword("Doctor123!"),
        Role = UserRole.Provider,
        CreatedAt = DateTime.UtcNow
    };
    context.Users.Add(providerUser);
    
    var testProvider = new Provider
    {
        Id = Guid.NewGuid(),
        UserId = providerUser.Id,
        Name = "Dr. Test",
        Specialty = "Endocrinologist",
        HospitalName = "Test Hospital",
        CreatedAt = DateTime.UtcNow
    };
    context.Providers.Add(testProvider);
    
    context.SaveChanges();
    
    Console.WriteLine("✅ Test data seeded!");
    Console.WriteLine("📧 Test Patient: test@healthquest.dk / Test123!");
    Console.WriteLine("📧 Test Provider: doctor@healthquest.dk / Doctor123!");
}

// Configure the HTTP request pipeline
app.UseSwagger();
app.UseSwaggerUI();

app.UseCors("AllowAngular");

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

Console.WriteLine("🚀 HealthQuest API is running!");
Console.WriteLine("📖 Swagger UI: http://localhost:5000/swagger");
Console.WriteLine("🎮 Ready to test!");

app.Run();
