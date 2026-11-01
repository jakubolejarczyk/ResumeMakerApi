using ResumeMakerApi.Database.Context;
using ResumeMakerApi.Database.Model;
using ResumeMakerApi.Models.Database.Base;
using ResumeMakerApi.Models.Database.Data;

namespace ResumeMakerApi.Services.Database;

public class EducationDatabaseService(DatabaseContext context)
{
    public void Create(BaseDatabaseModel<EducationDataDatabaseModel> userToCreate)
    {
        var resume = context.Resumes.FirstOrDefault(resume => resume.Id == userToCreate.Metadata.ParentId);
        var newEducation = new EducationDatabaseModel
        {
            ParentId = userToCreate.Metadata.ParentId,
            Order = userToCreate.Metadata.Order,
            IsSelected = userToCreate.Metadata.IsSelected,
            InstitutionName = userToCreate.Data.InstitutionName,
            FieldOfStudy = userToCreate.Data.FieldOfStudy,
            Degree = userToCreate.Data.Degree,
            StartYear = userToCreate.Data.StartYear,
            EndYear = userToCreate.Data.EndYear,
            Resume = resume ?? throw new Exception($"Resume with ID {userToCreate.Metadata.ParentId} not found.")
        };
        context.Educations.Add(newEducation);
        context.SaveChanges();
    }

    public List<BaseDatabaseModel<EducationDataDatabaseModel>> ReadAll()
    {
        return context.Educations.Select(user => new BaseDatabaseModel<EducationDataDatabaseModel>
        {
            Data = new EducationDataDatabaseModel
            {
                InstitutionName = user.InstitutionName,
                FieldOfStudy = user.FieldOfStudy,
                Degree = user.Degree,
                StartYear = user.StartYear,
                EndYear = user.EndYear
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

    public BaseDatabaseModel<EducationDataDatabaseModel>? ReadById(int id)
    {
        return context.Educations.Where(user => user.Id == id).Select(user => new BaseDatabaseModel<EducationDataDatabaseModel>
        {
            Metadata = new BaseDatabaseMetadataModel
            {
                Id = user.Id,
                ParentId = user.ParentId,
                Order = user.Order,
                IsSelected = user.IsSelected
            },
            Data = new EducationDataDatabaseModel
            {
                InstitutionName = user.InstitutionName,
                FieldOfStudy = user.FieldOfStudy,
                Degree = user.Degree,
                StartYear = user.StartYear,
                EndYear = user.EndYear
            }
        }).FirstOrDefault();
    }

    public List<BaseDatabaseModel<EducationDataDatabaseModel>> ReadByParentId(int parentId)
    {
        return context.Educations.Where(user => user.ParentId == parentId).Select(user => new BaseDatabaseModel<EducationDataDatabaseModel>
        {
            Metadata = new BaseDatabaseMetadataModel
            {
                Id = user.Id,
                ParentId = user.ParentId,
                Order = user.Order,
                IsSelected = user.IsSelected
            },
            Data = new EducationDataDatabaseModel
            {
                InstitutionName = user.InstitutionName,
                FieldOfStudy = user.FieldOfStudy,
                Degree = user.Degree,
                StartYear = user.StartYear,
                EndYear = user.EndYear
            }
        }).ToList();
    }

    public void Update(BaseDatabaseModel<EducationDataDatabaseModel> userToUpdate)
    {
        var user = context.Educations.FirstOrDefault(user => user.Id == userToUpdate.Metadata.Id);
        if (user == null)
        {
            throw new Exception($"Education with ID {userToUpdate.Metadata.Id} not found.");
        }
        user.ParentId = userToUpdate.Metadata.ParentId;
        user.Order = userToUpdate.Metadata.Order;
        user.IsSelected = userToUpdate.Metadata.IsSelected;
        user.InstitutionName = userToUpdate.Data.InstitutionName;
        user.FieldOfStudy = userToUpdate.Data.FieldOfStudy;
        user.Degree = userToUpdate.Data.Degree;
        user.StartYear = userToUpdate.Data.StartYear;
        user.EndYear = userToUpdate.Data.EndYear;
        context.Educations.Update(user);
        context.SaveChanges();
    }

    public void Delete(int id)
    {
        context.Educations.RemoveRange(context.Educations.Where(user => user.Id == id));
        context.SaveChanges();
    }

    public List<BaseDatabaseModel<EducationDataDatabaseModel>> GetEducations()
    {
        return context.Educations.Select(user => new BaseDatabaseModel<EducationDataDatabaseModel>
        {
            Data = new EducationDataDatabaseModel
            {
                InstitutionName = user.InstitutionName,
                FieldOfStudy = user.FieldOfStudy,
                Degree = user.Degree,
                StartYear = user.StartYear,
                EndYear = user.EndYear
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
