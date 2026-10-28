namespace ResumeMakerApi.Models.Database.Base;

public class BaseDatabaseModel<TData>
{
    public required TData Data { get; set; }

    public required BaseDatabaseMetadataModel Metadata { get; set; }
}
