using ResumeMakerApi.Consts.Database;
using ResumeMakerApi.Models.Database.Base;
using ResumeMakerApi.Models.Database.Data;

namespace ResumeMakerApi.Data.Database;

public class UserDatabaseData
{
    private readonly List<BaseDatabaseModel<UserDataDatabaseModel>> Users = [];

    public UserDatabaseData(UserDatabaseConst userDatabaseConst)
    {
        Users.Clear();
        Users.Add(userDatabaseConst.USER_0);
    }

    public void SetUsers(List<BaseDatabaseModel<UserDataDatabaseModel>> users)
    {
        Users.Clear();
        Users.AddRange(users);
    }

    public List<BaseDatabaseModel<UserDataDatabaseModel>> GetUsers()
    {
        return [..Users];
    }
}
