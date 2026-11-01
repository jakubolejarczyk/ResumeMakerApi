namespace ResumeMakerApi.Database.Model;

public class EducationDatabaseModel
{
    public required int Id { get; set; }

    public int? ParentId { get; set; }

    public required int Order { get; set; }

    public required bool IsSelected { get; set; }

    public required string InstitutionName { get; set; }

    public required string FieldOfStudy { get; set; }

    public required string Degree { get; set; }

    public required int StartYear { get; set; }

    public required int EndYear { get; set; }
}
