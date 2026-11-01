using ResumeMakerApi.Data.Database;
using ResumeMakerApi.Models.Database.Base;
using ResumeMakerApi.Models.Database.Data;

namespace ResumeMakerApi.Services.Database;

public class ExperienceDatabaseService(ExperienceDatabaseData experienceDatabaseData)
{
    public void Create(BaseDatabaseModel<ExperienceDataDatabaseModel> experienceToCreate)
    {
        var experiences = ReadAll();
        experiences.Add(experienceToCreate);
        experienceDatabaseData.SetExperiences(experiences);
    }

    public List<BaseDatabaseModel<ExperienceDataDatabaseModel>> ReadAll()
    {
        return experienceDatabaseData.GetExperiences();
    }

    public BaseDatabaseModel<ExperienceDataDatabaseModel>? ReadById(int id)
    {
        return ReadAll().Find(experience => experience.Metadata.Id == id);
    }

    public List<BaseDatabaseModel<ExperienceDataDatabaseModel>> ReadByParentId(int parentId)
    {
        return [..ReadAll().Where(experience =>
        {
            if (experience.Metadata.ParentId == null)
            {
                return false;
            }
            return experience.Metadata.ParentId == parentId;
        })];
    }

    public void Update(BaseDatabaseModel<ExperienceDataDatabaseModel> experienceToUpdate)
    {
        var experiences = ReadAll().Select((experience) =>
        {
            if (experience.Metadata.Id == experienceToUpdate.Metadata.Id)
            {
                return experienceToUpdate;
            }
            return experience;
        }).ToList();
        experienceDatabaseData.SetExperiences(experiences);
    }

    public void Delete(int id)
    {
        var experiences = experienceDatabaseData.GetExperiences()
            .Where(experience => experience.Metadata.Id != id)
            .ToList();
        experienceDatabaseData.SetExperiences(experiences);
    }

    public List<BaseDatabaseModel<ExperienceDataDatabaseModel>> GetExperiences()
    {
        return experienceDatabaseData.GetExperiences();
    }
}
