using ResumeMakerApi.Data.Database;
using ResumeMakerApi.Models.Database.Base;
using ResumeMakerApi.Models.Database.Data;

namespace ResumeMakerApi.Services.Database;

public class UserDatabaseService(UserDatabaseData userDatabaseData)
{
    public void Create(BaseDatabaseModel<UserDataDatabaseModel> userToCreate)
    {
        var users = ReadAll();
        users.Add(userToCreate);
        userDatabaseData.SetUsers(users);
    }

    public List<BaseDatabaseModel<UserDataDatabaseModel>> ReadAll()
    {
        return userDatabaseData.GetUsers();
    }

    public BaseDatabaseModel<UserDataDatabaseModel>? ReadById(int id)
    {
        return ReadAll().Find(user => user.Metadata.Id == id);
    }

    public List<BaseDatabaseModel<UserDataDatabaseModel>> ReadByParentId(int parentId)
    {
        return [..ReadAll().Where(user =>
        {
            if (user.Metadata.ParentId == null)
            {
                return false;
            }
            return user.Metadata.ParentId == parentId;
        })];
    }

    public void Update(BaseDatabaseModel<UserDataDatabaseModel> userToUpdate)
    {
        var users = ReadAll().Select((user) =>
        {
            if (user.Metadata.Id == userToUpdate.Metadata.Id)
            {
                return userToUpdate;
            }
            return user;
        }).ToList();
        userDatabaseData.SetUsers(users);
    }

    public void Delete(int id)
    {
        var users = userDatabaseData.GetUsers()
            .Where(user => user.Metadata.Id != id)
            .ToList();
        userDatabaseData.SetUsers(users);
    }

    public List<BaseDatabaseModel<UserDataDatabaseModel>> GetUsers()
    {
        return userDatabaseData.GetUsers();
    }
}
