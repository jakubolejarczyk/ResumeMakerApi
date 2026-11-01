namespace ResumeMakerApi.Models.Database.Data;

public class UserDataDatabaseModel
{
    public required string FirstName { get; set; }

    public required string LastName { get; set; }

    public required string PhoneNumber { get; set; }

    public required string Email { get; set; }

    public required string City { get; set; }

    public required string Country { get; set; }

    public required string[] Links { get; set; }
}
