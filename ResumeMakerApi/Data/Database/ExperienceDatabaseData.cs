using ResumeMakerApi.Consts.Database;
using ResumeMakerApi.Models.Database.Base;
using ResumeMakerApi.Models.Database.Data;

namespace ResumeMakerApi.Data.Database;

public class ExperienceDatabaseData
{
    private readonly List<BaseDatabaseModel<ExperienceDataDatabaseModel>> Experiences = [];

    public ExperienceDatabaseData(ExperienceDatabaseConst experienceDatabaseConst)
    {
        Experiences.Clear();
        Experiences.Add(experienceDatabaseConst.EXPERIENCE_0);
        Experiences.Add(experienceDatabaseConst.EXPERIENCE_1);
    }

    public void SetExperiences(List<BaseDatabaseModel<ExperienceDataDatabaseModel>> experiences)
    {
        Experiences.Clear();
        Experiences.AddRange(experiences);
    }

    public List<BaseDatabaseModel<ExperienceDataDatabaseModel>> GetExperiences()
    {
        return Experiences;
    }
}
