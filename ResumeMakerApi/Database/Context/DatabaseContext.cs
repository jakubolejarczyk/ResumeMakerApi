using Microsoft.EntityFrameworkCore;
using ResumeMakerApi.Database.Model;

namespace ResumeMakerApi.Database.Context;

public class DatabaseContext(DbContextOptions<DatabaseContext> options) : DbContext(options)
{
    public DbSet<EducationDatabaseModel> Educations => Set<EducationDatabaseModel>();

    public DbSet<ExperienceDatabaseModel> Experiences => Set<ExperienceDatabaseModel>();

    public DbSet<ResumeDatabaseModel> Resumes => Set<ResumeDatabaseModel>();

    public DbSet<SkillDatabaseModel> Skills => Set<SkillDatabaseModel>();

    public DbSet<UserDatabaseModel> Users => Set<UserDatabaseModel>();
}
