using ResumeMakerApi.Consts.Database;
using ResumeMakerApi.Models.Database.Base;
using ResumeMakerApi.Models.Database.Data;

namespace ResumeMakerApi.Data.Database;

public class UserDatabaseData(UserDatabaseConst userDatabaseConst)
{
    private readonly List<BaseDatabaseModel<UserDataDatabaseModel>> Users = [];

    public void Init()
    {
        Users.Clear();
        Users.Add(userDatabaseConst.USER_0);
    }

    public void Set(List<BaseDatabaseModel<UserDataDatabaseModel>> users)
    {
        Users.Clear();
        Users.AddRange(users);
    }

    public List<BaseDatabaseModel<UserDataDatabaseModel>> Get()
    {
        return Users;
    }
}
