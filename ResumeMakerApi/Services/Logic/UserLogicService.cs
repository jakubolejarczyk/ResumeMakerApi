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
                Message = "Failed to select the user because it does not exist.",
                Body = null
            };
        }
        var parentId = userToSelect.Metadata.ParentId;
        var users = parentId.HasValue
            ? service.ReadByParentId(parentId.Value)
            : service.ReadAll();
        foreach (var user in users.ToList())
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
            Message = "The user was selected correctly.",
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
                Message = "Failed to deselect the user because it does not exist.",
                Body = null
            };
        }
        userToDeselect.Metadata.IsSelected = false;
        service.Update(userToDeselect);
        return new ResponseLogicModel<BaseDatabaseModel<UserDataDatabaseModel>>
        {
            Success = true,
            Message = "The user was deselected correctly.",
            Body = userToDeselect
        };
    }

    public ResponseLogicModel<BaseDatabaseModel<UserDataDatabaseModel>> Create(UserDataDatabaseModel data, int? parentId)
    {
        var users = parentId.HasValue
            ? service.ReadByParentId(parentId.Value)
            : service.ReadAll();
        var lastOrder = users.Count == 0 ? 0 : users.Max(user => user.Metadata.Order);
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
            Message = "The user was created correctly.",
            Body = userToCreate
        };
    }

    public ResponseLogicModel<BaseDatabaseModel<UserDataDatabaseModel>> ReadById(int id)
    {
        var user = service.ReadById(id);
        if (user == null)
        {
            return new ResponseLogicModel<BaseDatabaseModel<UserDataDatabaseModel>>
            {
                Success = false,
                Message = "Failed to read the user.",
                Body = null
            };
        }
        return new ResponseLogicModel<BaseDatabaseModel<UserDataDatabaseModel>>
        {
            Success = true,
            Message = "The user was read correctly.",
            Body = user
        };
    }

    public ResponseLogicModel<List<BaseDatabaseModel<UserDataDatabaseModel>>> ReadByParentId(int? parentId)
    {
        var users = parentId.HasValue
            ? service.ReadByParentId(parentId.Value)
            : service.ReadAll();
        if (users.Count == 0)
        {
            return new ResponseLogicModel<List<BaseDatabaseModel<UserDataDatabaseModel>>>
            {
                Success = false,
                Message = "The list of users is empty.",
                Body = []
            };
        }
        return new ResponseLogicModel<List<BaseDatabaseModel<UserDataDatabaseModel>>>
        {
            Success = true,
            Message = "The users was read correctly.",
            Body = users
        };
    }

    public ResponseLogicModel<BaseDatabaseModel<UserDataDatabaseModel>> Update(int id, UserDataDatabaseModel data)
    {
        var userToUpdate = service.ReadById(id);
        if (userToUpdate == null)
        {
            return new ResponseLogicModel<BaseDatabaseModel<UserDataDatabaseModel>>
            {
                Success = false,
                Message = "The list of users is empty.",
                Body = null
            };
        }
        userToUpdate.Data = data;
        service.Update(userToUpdate);
        return new ResponseLogicModel<BaseDatabaseModel<UserDataDatabaseModel>>
        {
            Success = true,
            Message = "The user was updated correctly.",
            Body = userToUpdate
        };
    }

    public ResponseLogicModel<BaseDatabaseModel<UserDataDatabaseModel>> Delete(int id)
    {
        var userToDelete = service.ReadById(id);
        if (userToDelete == null)
        {
            return new ResponseLogicModel<BaseDatabaseModel<UserDataDatabaseModel>>
            {
                Success = false,
                Message = "Failed to delete the user because it does not exist.",
                Body = null
            };
        }
        var parentId = userToDelete.Metadata.ParentId;
        service.Delete(userToDelete.Metadata.Id);
        var usersToUpdate = parentId.HasValue
            ? service.ReadByParentId(parentId.Value)
            : service.ReadAll();
        usersToUpdate
            .OrderBy(item => item.Metadata.Order)
            .Select((itemToUpdate, index) =>
            {
                itemToUpdate.Metadata.Order = index;
                return itemToUpdate;
            })
            .ToList()
            .ForEach(service.Update);
        return new ResponseLogicModel<BaseDatabaseModel<UserDataDatabaseModel>>
        {
            Success = true,
            Message = "The user was deleted correctly.",
            Body = userToDelete
        };
    }

    public ResponseLogicModel<BaseDatabaseModel<UserDataDatabaseModel>> MoveUp(int id)
    {
        var userToMoveUp = service.ReadById(id);
        if (userToMoveUp == null)
        {
            return new ResponseLogicModel<BaseDatabaseModel<UserDataDatabaseModel>>
            {
                Success = false,
                Message = "Failed to move up the user because it does not exist.",
                Body = null
            };
        }
        if (userToMoveUp.Metadata.Order == 0)
        {
            return new ResponseLogicModel<BaseDatabaseModel<UserDataDatabaseModel>>
            {
                Success = false,
                Message = "Failed to move up the user because it is the first element.",
                Body = null
            };
        }
        var users = userToMoveUp.Metadata.ParentId.HasValue
            ? service.ReadByParentId(userToMoveUp.Metadata.ParentId.Value)
            : service.ReadAll();
        var prevUser = users.FirstOrDefault(
            item => item.Metadata.Order == userToMoveUp.Metadata.Order - 1
        );
        if (prevUser == null)
        {
            return new ResponseLogicModel<BaseDatabaseModel<UserDataDatabaseModel>>
            {
                Success = false,
                Message = "Failed to move up the user because the previous user does not exist.",
                Body = null
            };
        }
        var userToMoveUpOrder = userToMoveUp.Metadata.Order;
        userToMoveUp.Metadata.Order = prevUser.Metadata.Order;
        prevUser.Metadata.Order = userToMoveUpOrder;
        service.Update(userToMoveUp);
        service.Update(prevUser);
        return new ResponseLogicModel<BaseDatabaseModel<UserDataDatabaseModel>>
        {
            Success = true,
            Message = "The user was moved up correctly.",
            Body = userToMoveUp
        };
    }

    public ResponseLogicModel<BaseDatabaseModel<UserDataDatabaseModel>> MoveDown(int id)
    {
        var userToMoveDown = service.ReadById(id);
        if (userToMoveDown == null)
        {
            return new ResponseLogicModel<BaseDatabaseModel<UserDataDatabaseModel>>
            {
                Success = false,
                Message = "Failed to move down the user because it does not exist.",
                Body = null
            };
        }
        var users = userToMoveDown.Metadata.ParentId.HasValue
            ? service.ReadByParentId(userToMoveDown.Metadata.ParentId.Value)
            : service.ReadAll();
        var lastOrder = users.Aggregate(
            0,
            (acc, curr) => curr.Metadata.Order > acc
                ? curr.Metadata.Order
                : acc
        );
        if (userToMoveDown.Metadata.Order == lastOrder)
        {
            return new ResponseLogicModel<BaseDatabaseModel<UserDataDatabaseModel>>
            {
                Success = false,
                Message = "Failed to move down the user because it is the last element.",
                Body = null
            };
        }
        var nextUser = users.FirstOrDefault(
            user => user.Metadata.Order == userToMoveDown.Metadata.Order + 1
        );
        if (nextUser == null)
        {
            return new ResponseLogicModel<BaseDatabaseModel<UserDataDatabaseModel>>
            {
                Success = false,
                Message = "Failed to move down the user because the next user does not exist.",
                Body = null
            };
        }
        var userToMoveDownOrder = userToMoveDown.Metadata.Order;
        userToMoveDown.Metadata.Order = nextUser.Metadata.Order;
        nextUser.Metadata.Order = userToMoveDownOrder;
        service.Update(userToMoveDown);
        service.Update(nextUser);
        return new ResponseLogicModel<BaseDatabaseModel<UserDataDatabaseModel>>
        {
            Success = true,
            Message = "The user was moved down correctly.",
            Body = userToMoveDown
        };
    }
}
