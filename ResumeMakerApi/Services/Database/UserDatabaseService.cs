using ResumeMakerApi.Database.Context;
using ResumeMakerApi.Database.Model;
using ResumeMakerApi.Models.Database.Base;
using ResumeMakerApi.Models.Database.Data;

namespace ResumeMakerApi.Services.Database;

public class UserDatabaseService(DatabaseContext context)
{
    public void Create(BaseDatabaseModel<UserDataDatabaseModel> userToCreate)
    {
        var newUser = new UserDatabaseModel
        {
            ParentId = userToCreate.Metadata.ParentId,
            Order = userToCreate.Metadata.Order,
            IsSelected = userToCreate.Metadata.IsSelected,
            FirstName = userToCreate.Data.FirstName,
            LastName = userToCreate.Data.LastName,
            PhoneNumber = userToCreate.Data.PhoneNumber,
            Email = userToCreate.Data.Email,
            City = userToCreate.Data.City,
            Country = userToCreate.Data.Country,
            Links = userToCreate.Data.Links
        };
        context.Users.Add(newUser);
        context.SaveChanges();
    }

    public List<BaseDatabaseModel<UserDataDatabaseModel>> ReadAll()
    {
        return context.Users.Select(user => new BaseDatabaseModel<UserDataDatabaseModel>
        {
            Data = new UserDataDatabaseModel
            {
                FirstName = user.FirstName,
                LastName = user.LastName,
                PhoneNumber = user.PhoneNumber,
                Email = user.Email,
                City = user.City,
                Country = user.Country,
                Links = user.Links
            },
            Metadata = new BaseDatabaseMetadataModel
            {
                Id = user.Id,
                ParentId = user.ParentId,
                Order = user.Order,
                IsSelected = user.IsSelected
            }
        }).ToList();
    }

    public BaseDatabaseModel<UserDataDatabaseModel>? ReadById(int id)
    {
        return context.Users.Where(user => user.Id == id).Select(user => new BaseDatabaseModel<UserDataDatabaseModel>
        {
            Metadata = new BaseDatabaseMetadataModel
            {
                Id = user.Id,
                ParentId = user.ParentId,
                Order = user.Order,
                IsSelected = user.IsSelected
            },
            Data = new UserDataDatabaseModel
            {
                FirstName = user.FirstName,
                LastName = user.LastName,
                PhoneNumber = user.PhoneNumber,
                Email = user.Email,
                City = user.City,
                Country = user.Country,
                Links = user.Links
            }
        }).FirstOrDefault();
    }

    public List<BaseDatabaseModel<UserDataDatabaseModel>> ReadByParentId(int parentId)
    {
        return context.Users.Where(user => user.ParentId == parentId).Select(user => new BaseDatabaseModel<UserDataDatabaseModel>
        {
            Metadata = new BaseDatabaseMetadataModel
            {
                Id = user.Id,
                ParentId = user.ParentId,
                Order = user.Order,
                IsSelected = user.IsSelected
            },
            Data = new UserDataDatabaseModel
            {
                FirstName = user.FirstName,
                LastName = user.LastName,
                PhoneNumber = user.PhoneNumber,
                Email = user.Email,
                City = user.City,
                Country = user.Country,
                Links = user.Links
            }
        }).ToList();
    }

    public void Update(BaseDatabaseModel<UserDataDatabaseModel> userToUpdate)
    {
        var user = context.Users.FirstOrDefault(user => user.Id == userToUpdate.Metadata.Id);
        if (user == null)
        {
            throw new Exception($"User with ID {userToUpdate.Metadata.Id} not found.");
        }
        user.ParentId = userToUpdate.Metadata.ParentId;
        user.Order = userToUpdate.Metadata.Order;
        user.IsSelected = userToUpdate.Metadata.IsSelected;
        user.FirstName = userToUpdate.Data.FirstName;
        user.LastName = userToUpdate.Data.LastName;
        user.PhoneNumber = userToUpdate.Data.PhoneNumber;
        user.Email = userToUpdate.Data.Email;
        user.City = userToUpdate.Data.City;
        user.Country = userToUpdate.Data.Country;
        user.Links = userToUpdate.Data.Links;
        context.Users.Update(user);
        context.SaveChanges();
    }

    public void Delete(int id)
    {
        context.Users.RemoveRange(context.Users.Where(user => user.Id == id));
        context.SaveChanges();
    }

    public List<BaseDatabaseModel<UserDataDatabaseModel>> GetUsers()
    {
        return context.Users.Select(user => new BaseDatabaseModel<UserDataDatabaseModel>
        {
            Data = new UserDataDatabaseModel
            {
                FirstName = user.FirstName,
                LastName = user.LastName,
                PhoneNumber = user.PhoneNumber,
                Email = user.Email,
                City = user.City,
                Country = user.Country,
                Links = user.Links
            },
            Metadata = new BaseDatabaseMetadataModel
            {
                Id = user.Id,
                ParentId = user.ParentId,
                Order = user.Order,
                IsSelected = user.IsSelected
            }
        }).ToList();
    }
}
