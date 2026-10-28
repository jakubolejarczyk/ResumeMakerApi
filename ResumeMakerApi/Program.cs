using ResumeMakerApi.Consts.Database;
using ResumeMakerApi.Data.Database;
using ResumeMakerApi.Services.Database;
using Scalar.AspNetCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

builder.Services.AddSingleton<UserDatabaseConst>();
builder.Services.AddSingleton<ResumeDatabaseConst>();
builder.Services.AddSingleton<SkillDatabaseConst>();
builder.Services.AddSingleton<ExperienceDatabaseConst>();
builder.Services.AddSingleton<EducationDatabaseConst>();

builder.Services.AddSingleton<UserDatabaseData>();
builder.Services.AddSingleton<SkillDatabaseData>();
builder.Services.AddSingleton<ResumeDatabaseData>();
builder.Services.AddSingleton<ExperienceDatabaseData>();
builder.Services.AddSingleton<EducationDatabaseData>();

builder.Services.AddSingleton<UserDatabaseService>();
builder.Services.AddSingleton<SkillDatabaseService>();
builder.Services.AddSingleton<ResumeDatabaseService>();
builder.Services.AddSingleton<ExperienceDatabaseService>();
builder.Services.AddSingleton<EducationDatabaseService>();

var app = builder.Build();

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
