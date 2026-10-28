namespace ResumeMakerApi.Models.Database.Data;

public class ExperienceDataDatabaseModel
{
    public required string Company { get; set; }

    public required string JobTitle { get; set; }

    public required string[] Description { get; set; }

    public required string StartDate { get; set; }

    public string? EndDate { get; set; }
}
