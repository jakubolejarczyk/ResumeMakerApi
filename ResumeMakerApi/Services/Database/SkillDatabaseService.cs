using ResumeMakerApi.Database.Context;
using ResumeMakerApi.Database.Model;
using ResumeMakerApi.Models.Database.Base;
using ResumeMakerApi.Models.Database.Data;

namespace ResumeMakerApi.Services.Database;

public class SkillDatabaseService(DatabaseContext context)
{
    public void Create(BaseDatabaseModel<SkillDataDatabaseModel> userToCreate)
    {
        var resume = context.Resumes.FirstOrDefault(resume => resume.Id == userToCreate.Metadata.ParentId);
        var newSkill = new SkillDatabaseModel
        {
            ParentId = userToCreate.Metadata.ParentId,
            Order = userToCreate.Metadata.Order,
            IsSelected = userToCreate.Metadata.IsSelected,
            Category = userToCreate.Data.Category,
            Skills = userToCreate.Data.Skills,
            Resume = resume ?? throw new Exception($"Resume with ID {userToCreate.Metadata.ParentId} not found.")
        };
        context.Skills.Add(newSkill);
        context.SaveChanges();
    }

    public List<BaseDatabaseModel<SkillDataDatabaseModel>> ReadAll()
    {
        return context.Skills.Select(user => new BaseDatabaseModel<SkillDataDatabaseModel>
        {
            Data = new SkillDataDatabaseModel
            {
                Category = user.Category,
                Skills = user.Skills
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

    public BaseDatabaseModel<SkillDataDatabaseModel>? ReadById(int id)
    {
        return context.Skills.Where(user => user.Id == id).Select(user => new BaseDatabaseModel<SkillDataDatabaseModel>
        {
            Metadata = new BaseDatabaseMetadataModel
            {
                Id = user.Id,
                ParentId = user.ParentId,
                Order = user.Order,
                IsSelected = user.IsSelected
            },
            Data = new SkillDataDatabaseModel
            {
                Category = user.Category,
                Skills = user.Skills
            }
        }).FirstOrDefault();
    }

    public List<BaseDatabaseModel<SkillDataDatabaseModel>> ReadByParentId(int parentId)
    {
        return context.Skills.Where(user => user.ParentId == parentId).Select(user => new BaseDatabaseModel<SkillDataDatabaseModel>
        {
            Metadata = new BaseDatabaseMetadataModel
            {
                Id = user.Id,
                ParentId = user.ParentId,
                Order = user.Order,
                IsSelected = user.IsSelected
            },
            Data = new SkillDataDatabaseModel
            {
                Category = user.Category,
                Skills = user.Skills
            }
        }).ToList();
    }

    public void Update(BaseDatabaseModel<SkillDataDatabaseModel> userToUpdate)
    {
        var user = context.Skills.FirstOrDefault(user => user.Id == userToUpdate.Metadata.Id);
        if (user == null)
        {
            throw new Exception($"Skill with ID {userToUpdate.Metadata.Id} not found.");
        }
        user.ParentId = userToUpdate.Metadata.ParentId;
        user.Order = userToUpdate.Metadata.Order;
        user.IsSelected = userToUpdate.Metadata.IsSelected;
        user.Category = userToUpdate.Data.Category;
        user.Skills = userToUpdate.Data.Skills;
        context.Skills.Update(user);
        context.SaveChanges();
    }

    public void Delete(int id)
    {
        context.Skills.RemoveRange(context.Skills.Where(user => user.Id == id));
        context.SaveChanges();
    }

    public List<BaseDatabaseModel<SkillDataDatabaseModel>> GetSkills()
    {
        return context.Skills.Select(user => new BaseDatabaseModel<SkillDataDatabaseModel>
        {
            Data = new SkillDataDatabaseModel
            {
                Category = user.Category,
                Skills = user.Skills
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
