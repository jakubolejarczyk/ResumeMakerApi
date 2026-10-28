using ResumeMakerApi.Models.Database.Base;
using ResumeMakerApi.Models.Database.Data;
using ResumeMakerApi.Models.Logic;
using ResumeMakerApi.Services.Database;

namespace ResumeMakerApi.Services.Logic;

public class UserLogicService(UserDatabaseService service)
{
    private int NextId = service.GetUsers().Count;

    public ResponseLogicModel<BaseDatabaseModel<UserDataDatabaseModel>> Select(int id)
    {
        var userToSelect = service.ReadById(id);
        if (userToSelect == null)
        {
            return new ResponseLogicModel<BaseDatabaseModel<UserDataDatabaseModel>>
            {
                Success = false,
                Message = "Failed to select the item because it does not exist.",
                Body = null
            };
        }
        var parentId = userToSelect.Metadata.ParentId;
        var users = parentId.HasValue
            ? service.ReadByParentId(parentId.Value)
            : service.ReadAll();
        foreach (var user in users)
        {
            if (user.Metadata.Id == userToSelect.Metadata.Id)
            {
                user.Metadata.IsSelected = true;
            }
            else
            {
                user.Metadata.IsSelected = false;
            }
            service.Update(user);
        }
        var currentUser = service.ReadById(id);
        return new ResponseLogicModel<BaseDatabaseModel<UserDataDatabaseModel>>
        {
            Success = true,
            Message = "The item was selected correctly.",
            Body = currentUser
        };
    }

    public ResponseLogicModel<BaseDatabaseModel<UserDataDatabaseModel>> Deselect(int id)
    {
        var userToDeselect = service.ReadById(id);
        if (userToDeselect == null)
        {
            return new ResponseLogicModel<BaseDatabaseModel<UserDataDatabaseModel>>
            {
                Success = false,
                Message = "Failed to deselect the item because it does not exist.",
                Body = null
            };
        }
        userToDeselect.Metadata.IsSelected = false;
        service.Update(userToDeselect);
        return new ResponseLogicModel<BaseDatabaseModel<UserDataDatabaseModel>>
        {
            Success = true,
            Message = "The item was deselected correctly.",
            Body = userToDeselect
        };
    }

    public ResponseLogicModel<BaseDatabaseModel<UserDataDatabaseModel>> Create(UserDataDatabaseModel data, int? parentId)
    {
        var users = parentId.HasValue
            ? service.ReadByParentId(parentId.Value)
            : service.ReadAll();
        var lastOrder = users.Max(user => user.Metadata.Order);
        var userToCreate = new BaseDatabaseModel<UserDataDatabaseModel>
        {
            Data = data,
            Metadata = new BaseDatabaseMetadataModel
            {
                Id = NextId,
                ParentId = parentId,
                Order = lastOrder + 1,
                IsSelected = false
            }
        };
        NextId++;
        service.Create(userToCreate);
        return new ResponseLogicModel<BaseDatabaseModel<UserDataDatabaseModel>>
        {
            Success = true,
            Message = "The item was created correctly.",
            Body = userToCreate
        };
    }

    // TODO: Implement the below logics
    public void ReadById()
    {
        throw new NotImplementedException();
    }

    public void ReadByParentId()
    {
        throw new NotImplementedException();
    }

    public void Update()
    {
        throw new NotImplementedException();
    }

    public void Delete()
    {
        throw new NotImplementedException();
    }

    public void MoveUp()
    {
        throw new NotImplementedException();
    }

    public void MoveDown()
    {
        throw new NotImplementedException();
    }
}
