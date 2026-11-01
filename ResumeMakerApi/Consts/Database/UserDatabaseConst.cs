using ResumeMakerApi.Models.Database.Base;
using ResumeMakerApi.Models.Database.Data;

namespace ResumeMakerApi.Consts.Database;

public class UserDatabaseConst
{
    public BaseDatabaseModel<UserDataDatabaseModel> USER_0 = new()
    {
        Data = new UserDataDatabaseModel
        {
            FirstName = "Jakub",
            LastName = "Olejarczyk",
            PhoneNumber = "+48 501 007 197",
            Email = "jakub.olejarczyk@outlook.com",
            City = "Kielce",
            Country = "Poland",
            Links = [
                "olejarczykjakub.com",
                "linkedin.com/in/jakub-olejarczyk",
                "github.com/jakubolejarczyk",
                "leetcode.com/u/JakubOlejarczyk"
            ]
        },
        Metadata = new BaseDatabaseMetadataModel
        {
            Id = 0,
            ParentId = null,
            Order = 0,
            IsSelected = false
        }
    };
}
