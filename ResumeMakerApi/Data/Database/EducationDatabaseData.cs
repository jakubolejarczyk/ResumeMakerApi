using ResumeMakerApi.Consts.Database;
using ResumeMakerApi.Models.Database.Base;
using ResumeMakerApi.Models.Database.Data;

namespace ResumeMakerApi.Data.Database;

public class EducationDatabaseData
{
    private readonly List<BaseDatabaseModel<EducationDatabaseModel>> Educations = [];

    public EducationDatabaseData(EducationDatabaseConst educationDatabaseConst)
    {
        Educations.Clear();
        Educations.Add(educationDatabaseConst.EDUCATION_0);
        Educations.Add(educationDatabaseConst.EDUCATION_1);
    }

    public void SetEducations(List<BaseDatabaseModel<EducationDatabaseModel>> educations)
    {
        Educations.Clear();
        Educations.AddRange(educations);
    }

    public List<BaseDatabaseModel<EducationDatabaseModel>> GetEducations()
    {
        return Educations;
    }
}
