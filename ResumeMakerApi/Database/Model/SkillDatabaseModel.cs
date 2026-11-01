namespace ResumeMakerApi.Database.Model;

public class SkillDatabaseModel
{
    public required int Id { get; set; }

    public int? ParentId { get; set; }

    public required int Order { get; set; }

    public required bool IsSelected { get; set; }

    public required string Category { get; set; }

    public required string[] Skills { get; set; }
}
