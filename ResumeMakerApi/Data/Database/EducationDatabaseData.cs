using ResumeMakerApi.Consts.Database;
using ResumeMakerApi.Models.Database.Base;
using ResumeMakerApi.Models.Database.Data;

namespace ResumeMakerApi.Data.Database;

public class EducationDatabaseData(EducationDatabaseConst educationDatabaseConst)
{
    private readonly List<BaseDatabaseModel<EducationDataDatabaseModel>> Educations = [];

    public void Init()
    {
        Educations.Clear();
        Educations.Add(educationDatabaseConst.EDUCATION_0);
        Educations.Add(educationDatabaseConst.EDUCATION_1);
    }

    public void Set(List<BaseDatabaseModel<EducationDataDatabaseModel>> educations)
    {
        Educations.Clear();
        Educations.AddRange(educations);
    }

    public List<BaseDatabaseModel<EducationDataDatabaseModel>> Get()
    {
        return Educations;
    }
}
