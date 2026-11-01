using System.ComponentModel.DataAnnotations.Schema;

namespace ResumeMakerApi.Database.Model;

public class ResumeDatabaseModel
{
    public required int Id { get; set; }

    public int? ParentId { get; set; }

    public required int Order { get; set; }

    public required bool IsSelected { get; set; }

    public required string Name { get; set; }

    public required string JobTitle { get; set; }

    public required string Summary { get; set; }

    public required string Rodo { get; set; }

    [ForeignKey("UserId")]
    public required UserDatabaseModel User { get; set; }

    public required ICollection<EducationDatabaseModel> Educations { get; set; }

    public required ICollection<ExperienceDatabaseModel> Experiences { get; set; }

    public required ICollection<SkillDatabaseModel> Skills { get; set; }
}
