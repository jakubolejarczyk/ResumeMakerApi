namespace ResumeMakerApi.Models.Database.Base;

public class BaseDatabaseMetadataModel
{
    public required int Id { get; set; }

    public int? ParentId { get; set; }

    public required int Order { get; set; }

    public required bool IsSelected { get; set; }
}
