namespace ResumeMakerApi.Database.Model;

public class UserDatabaseModel
{
    public int Id { get; set; }

    public int? ParentId { get; set; }

    public required int Order { get; set; }

    public required bool IsSelected { get; set; }

    public required string FirstName { get; set; }

    public required string LastName { get; set; }

    public required string PhoneNumber { get; set; }

    public required string Email { get; set; }

    public required string[] Links { get; set; }

    public ICollection<ResumeDatabaseModel> Resumes { get; set; }
}
