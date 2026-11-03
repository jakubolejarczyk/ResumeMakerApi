using Microsoft.EntityFrameworkCore;
using ResumeMakerApi.Consts.Database;
using ResumeMakerApi.Data.Database;
using ResumeMakerApi.Database.Context;
using ResumeMakerApi.Services.Database;
using ResumeMakerApi.Services.Logic;
using Scalar.AspNetCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

builder.Services.AddDbContext<DatabaseContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddScoped<UserDatabaseConst>();
builder.Services.AddScoped<ResumeDatabaseConst>();
builder.Services.AddScoped<SkillDatabaseConst>();
builder.Services.AddScoped<ExperienceDatabaseConst>();
builder.Services.AddScoped<EducationDatabaseConst>();

builder.Services.AddScoped<UserDatabaseData>();
builder.Services.AddScoped<SkillDatabaseData>();
builder.Services.AddScoped<ResumeDatabaseData>();
builder.Services.AddScoped<ExperienceDatabaseData>();
builder.Services.AddScoped<EducationDatabaseData>();

builder.Services.AddScoped<UserDatabaseService>();
builder.Services.AddScoped<SkillDatabaseService>();
builder.Services.AddScoped<ResumeDatabaseService>();
builder.Services.AddScoped<ExperienceDatabaseService>();
builder.Services.AddScoped<EducationDatabaseService>();

builder.Services.AddScoped<UserLogicService>();
builder.Services.AddScoped<SkillLogicService>();
builder.Services.AddScoped<ResumeLogicService>();
builder.Services.AddScoped<ExperienceLogicService>();
builder.Services.AddScoped<EducationLogicService>();
builder.Services.AddScoped<PdfService>();

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", policy =>
    {
        policy
            .AllowAnyOrigin()
            .AllowAnyHeader()
            .AllowAnyMethod();
    });
});

var app = builder.Build();

app.UseCors("AllowAll");

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.MapScalarApiReference();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
