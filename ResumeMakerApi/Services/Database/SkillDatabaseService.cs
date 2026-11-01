using ResumeMakerApi.Data.Database;
using ResumeMakerApi.Models.Database.Base;
using ResumeMakerApi.Models.Database.Data;

namespace ResumeMakerApi.Services.Database;

public class SkillDatabaseService(SkillDatabaseData skillDatabaseData)
{
    public void Create(BaseDatabaseModel<SkillDataDatabaseModel> skillToCreate)
    {
        var skills = ReadAll();
        skills.Add(skillToCreate);
        skillDatabaseData.SetSkills(skills);
    }

    public List<BaseDatabaseModel<SkillDataDatabaseModel>> ReadAll()
    {
        return skillDatabaseData.GetSkills();
    }

    public BaseDatabaseModel<SkillDataDatabaseModel>? ReadById(int id)
    {
        return ReadAll().Find(skill => skill.Metadata.Id == id);
    }

    public List<BaseDatabaseModel<SkillDataDatabaseModel>> ReadByParentId(int parentId)
    {
        return [..ReadAll().Where(skill =>
        {
            if (skill.Metadata.ParentId == null)
            {
                return false;
            }
            return skill.Metadata.ParentId == parentId;
        })];
    }

    public void Update(BaseDatabaseModel<SkillDataDatabaseModel> skillToUpdate)
    {
        var skills = ReadAll().Select((skill) =>
        {
            if (skill.Metadata.Id == skillToUpdate.Metadata.Id)
            {
                return skillToUpdate;
            }
            return skill;
        }).ToList();
        skillDatabaseData.SetSkills(skills);
    }

    public void Delete(int id)
    {
        var skills = skillDatabaseData.GetSkills()
            .Where(skill => skill.Metadata.Id != id)
            .ToList();
        skillDatabaseData.SetSkills(skills);
    }

    public List<BaseDatabaseModel<SkillDataDatabaseModel>> GetSkills()
    {
        return skillDatabaseData.GetSkills();
    }
}
