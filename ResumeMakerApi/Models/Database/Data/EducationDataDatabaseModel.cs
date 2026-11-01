namespace ResumeMakerApi.Models.Database.Data;

public class EducationDatabaseModel
{
    public required string InstitutionName { get; set; }

    public required string FieldOfStudy { get; set; }

    public required string Degree { get; set; }

    public required int StartYear { get; set; }

    public required int EndYear { get; set; }
}
