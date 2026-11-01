namespace ResumeMakerApi.Database.Model;

public class ExperienceDatabaseModel
{
    public required int Id { get; set; }

    public int? ParentId { get; set; }

    public required int Order { get; set; }

    public required bool IsSelected { get; set; }

    public required string Company { get; set; }

    public required string JobTitle { get; set; }

    public required string[] Description { get; set; }

    public required string StartDate { get; set; }

    public string? EndDate { get; set; }
}
