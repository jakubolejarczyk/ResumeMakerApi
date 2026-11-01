using ResumeMakerApi.Database.Context;
using ResumeMakerApi.Database.Model;
using ResumeMakerApi.Models.Database.Base;
using ResumeMakerApi.Models.Database.Data;

namespace ResumeMakerApi.Services.Database;

public class ExperienceDatabaseService(DatabaseContext context)
{
    public void Create(BaseDatabaseModel<ExperienceDataDatabaseModel> userToCreate)
    {
        var resume = context.Resumes.FirstOrDefault(resume => resume.Id == userToCreate.Metadata.ParentId);
        var newExperience = new ExperienceDatabaseModel
        {
            ParentId = userToCreate.Metadata.ParentId,
            Order = userToCreate.Metadata.Order,
            IsSelected = userToCreate.Metadata.IsSelected,
            Company = userToCreate.Data.Company,
            JobTitle = userToCreate.Data.JobTitle,
            Description = userToCreate.Data.Description,
            StartDate = userToCreate.Data.StartDate,
            EndDate = userToCreate.Data.EndDate,
            Resume = resume ?? throw new Exception($"Resume with ID {userToCreate.Metadata.ParentId} not found.")
        };
        context.Experiences.Add(newExperience);
        context.SaveChanges();
    }

    public List<BaseDatabaseModel<ExperienceDataDatabaseModel>> ReadAll()
    {
        return context.Experiences.Select(user => new BaseDatabaseModel<ExperienceDataDatabaseModel>
        {
            Data = new ExperienceDataDatabaseModel
            {
                Company = user.Company,
                JobTitle = user.JobTitle,
                Description = user.Description,
                StartDate = user.StartDate,
                EndDate = user.EndDate
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

    public BaseDatabaseModel<ExperienceDataDatabaseModel>? ReadById(int id)
    {
        return context.Experiences.Where(user => user.Id == id).Select(user => new BaseDatabaseModel<ExperienceDataDatabaseModel>
        {
            Metadata = new BaseDatabaseMetadataModel
            {
                Id = user.Id,
                ParentId = user.ParentId,
                Order = user.Order,
                IsSelected = user.IsSelected
            },
            Data = new ExperienceDataDatabaseModel
            {
                Company = user.Company,
                JobTitle = user.JobTitle,
                Description = user.Description,
                StartDate = user.StartDate,
                EndDate = user.EndDate
            }
        }).FirstOrDefault();
    }

    public List<BaseDatabaseModel<ExperienceDataDatabaseModel>> ReadByParentId(int parentId)
    {
        return context.Experiences.Where(user => user.ParentId == parentId).Select(user => new BaseDatabaseModel<ExperienceDataDatabaseModel>
        {
            Metadata = new BaseDatabaseMetadataModel
            {
                Id = user.Id,
                ParentId = user.ParentId,
                Order = user.Order,
                IsSelected = user.IsSelected
            },
            Data = new ExperienceDataDatabaseModel
            {
                Company = user.Company,
                JobTitle = user.JobTitle,
                Description = user.Description,
                StartDate = user.StartDate,
                EndDate = user.EndDate
            }
        }).ToList();
    }

    public void Update(BaseDatabaseModel<ExperienceDataDatabaseModel> userToUpdate)
    {
        var user = context.Experiences.FirstOrDefault(user => user.Id == userToUpdate.Metadata.Id);
        if (user == null)
        {
            throw new Exception($"Experience with ID {userToUpdate.Metadata.Id} not found.");
        }
        user.ParentId = userToUpdate.Metadata.ParentId;
        user.Order = userToUpdate.Metadata.Order;
        user.IsSelected = userToUpdate.Metadata.IsSelected;
        user.Company = userToUpdate.Data.Company;
        user.JobTitle = userToUpdate.Data.JobTitle;
        user.Description = userToUpdate.Data.Description;
        user.StartDate = userToUpdate.Data.StartDate;
        user.EndDate = userToUpdate.Data.EndDate;
        context.Experiences.Update(user);
        context.SaveChanges();
    }

    public void Delete(int id)
    {
        context.Experiences.RemoveRange(context.Experiences.Where(user => user.Id == id));
        context.SaveChanges();
    }

    public List<BaseDatabaseModel<ExperienceDataDatabaseModel>> GetExperiences()
    {
        return context.Experiences.Select(user => new BaseDatabaseModel<ExperienceDataDatabaseModel>
        {
            Data = new ExperienceDataDatabaseModel
            {
                Company = user.Company,
                JobTitle = user.JobTitle,
                Description = user.Description,
                StartDate = user.StartDate,
                EndDate = user.EndDate
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
