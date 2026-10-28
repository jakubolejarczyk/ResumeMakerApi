namespace ResumeMakerApi.Models.Logic;

public class ResponseLogicModel<TBody>
{
    public required bool Success { get; set; }

    public required string Message { get; set; }

    public TBody? Body { get; set; }
}
