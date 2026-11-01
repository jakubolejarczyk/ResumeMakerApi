using ResumeMakerApi.Database.Context;
using ResumeMakerApi.Database.Model;
using ResumeMakerApi.Models.Database.Base;
using ResumeMakerApi.Models.Database.Data;

namespace ResumeMakerApi.Services.Database;

public class ResumeDatabaseService(DatabaseContext context)
{
    public void Create(BaseDatabaseModel<ResumeDataDatabaseModel> userToCreate)
    {
        var user = context.Users.FirstOrDefault(user => user.Id == userToCreate.Metadata.ParentId);
        var newResume = new ResumeDatabaseModel
        {
            ParentId = userToCreate.Metadata.ParentId,
            Order = userToCreate.Metadata.Order,
            IsSelected = userToCreate.Metadata.IsSelected,
            Name = userToCreate.Data.Name,
            JobTitle = userToCreate.Data.JobTitle,
            Summary = userToCreate.Data.Summary,
            Rodo = userToCreate.Data.Rodo,
            User = user ?? throw new Exception($"User with ID {userToCreate.Metadata.ParentId} not found.")
        };
        context.Resumes.Add(newResume);
        context.SaveChanges();
    }

    public List<BaseDatabaseModel<ResumeDataDatabaseModel>> ReadAll()
    {
        return context.Resumes.Select(user => new BaseDatabaseModel<ResumeDataDatabaseModel>
        {
            Data = new ResumeDataDatabaseModel
            {
                Name = user.Name,
                JobTitle = user.JobTitle,
                Summary = user.Summary,
                Rodo = user.Rodo
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

    public BaseDatabaseModel<ResumeDataDatabaseModel>? ReadById(int id)
    {
        return context.Resumes.Where(user => user.Id == id).Select(user => new BaseDatabaseModel<ResumeDataDatabaseModel>
        {
            Metadata = new BaseDatabaseMetadataModel
            {
                Id = user.Id,
                ParentId = user.ParentId,
                Order = user.Order,
                IsSelected = user.IsSelected
            },
            Data = new ResumeDataDatabaseModel
            {
                Name = user.Name,
                JobTitle = user.JobTitle,
                Summary = user.Summary,
                Rodo = user.Rodo
            }
        }).FirstOrDefault();
    }

    public List<BaseDatabaseModel<ResumeDataDatabaseModel>> ReadByParentId(int parentId)
    {
        return context.Resumes.Where(user => user.ParentId == parentId).Select(user => new BaseDatabaseModel<ResumeDataDatabaseModel>
        {
            Metadata = new BaseDatabaseMetadataModel
            {
                Id = user.Id,
                ParentId = user.ParentId,
                Order = user.Order,
                IsSelected = user.IsSelected
            },
            Data = new ResumeDataDatabaseModel
            {
                Name = user.Name,
                JobTitle = user.JobTitle,
                Summary = user.Summary,
                Rodo = user.Rodo
            }
        }).ToList();
    }

    public void Update(BaseDatabaseModel<ResumeDataDatabaseModel> userToUpdate)
    {
        var user = context.Resumes.FirstOrDefault(user => user.Id == userToUpdate.Metadata.Id);
        if (user == null)
        {
            throw new Exception($"Resume with ID {userToUpdate.Metadata.Id} not found.");
        }
        user.ParentId = userToUpdate.Metadata.ParentId;
        user.Order = userToUpdate.Metadata.Order;
        user.IsSelected = userToUpdate.Metadata.IsSelected;
        user.Name = userToUpdate.Data.Name;
        user.JobTitle = userToUpdate.Data.JobTitle;
        user.Summary = userToUpdate.Data.Summary;
        user.Rodo = userToUpdate.Data.Rodo;
        context.Resumes.Update(user);
        context.SaveChanges();
    }

    public void Delete(int id)
    {
        context.Resumes.RemoveRange(context.Resumes.Where(user => user.Id == id));
        context.SaveChanges();
    }

    public List<BaseDatabaseModel<ResumeDataDatabaseModel>> GetResumes()
    {
        return context.Resumes.Select(user => new BaseDatabaseModel<ResumeDataDatabaseModel>
        {
            Data = new ResumeDataDatabaseModel
            {
                Name = user.Name,
                JobTitle = user.JobTitle,
                Summary = user.Summary,
                Rodo = user.Rodo
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
