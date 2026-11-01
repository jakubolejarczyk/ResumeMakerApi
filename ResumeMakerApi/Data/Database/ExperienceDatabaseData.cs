using ResumeMakerApi.Consts.Database;
using ResumeMakerApi.Models.Database.Base;
using ResumeMakerApi.Models.Database.Data;

namespace ResumeMakerApi.Data.Database;

public class ExperienceDatabaseData
{
    private readonly List<BaseDatabaseModel<ExperienceDatabaseModel>> Experiences = [];

    public ExperienceDatabaseData(ExperienceDatabaseConst experienceDatabaseConst)
    {
        Experiences.Clear();
        Experiences.Add(experienceDatabaseConst.EXPERIENCE_0);
        Experiences.Add(experienceDatabaseConst.EXPERIENCE_1);
    }

    public void SetExperiences(List<BaseDatabaseModel<ExperienceDatabaseModel>> experiences)
    {
        Experiences.Clear();
        Experiences.AddRange(experiences);
    }

    public List<BaseDatabaseModel<ExperienceDatabaseModel>> GetExperiences()
    {
        return Experiences;
    }
}
