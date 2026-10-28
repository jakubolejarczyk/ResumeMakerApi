using ResumeMakerApi.Consts.Database;
using ResumeMakerApi.Models.Database.Base;
using ResumeMakerApi.Models.Database.Data;

namespace ResumeMakerApi.Data.Database;

public class ExperienceDatabaseData(ExperienceDatabaseConst experienceDatabaseConst)
{
    private readonly List<BaseDatabaseModel<ExperienceDataDatabaseModel>> Experiences = [];

    public void Init()
    {
        Experiences.Clear();
        Experiences.Add(experienceDatabaseConst.EXPERIENCE_0);
        Experiences.Add(experienceDatabaseConst.EXPERIENCE_1);
    }

    public void Set(List<BaseDatabaseModel<ExperienceDataDatabaseModel>> experiences)
    {
        Experiences.Clear();
        Experiences.AddRange(experiences);
    }

    public List<BaseDatabaseModel<ExperienceDataDatabaseModel>> Get()
    {
        return Experiences;
    }
}
