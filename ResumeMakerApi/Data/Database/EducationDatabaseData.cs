using ResumeMakerApi.Consts.Database;
using ResumeMakerApi.Models.Database.Base;
using ResumeMakerApi.Models.Database.Data;

namespace ResumeMakerApi.Data.Database;

public class EducationDatabaseData
{
    private readonly List<BaseDatabaseModel<EducationDataDatabaseModel>> Educations = [];

    public EducationDatabaseData(EducationDatabaseConst educationDatabaseConst)
    {
        Educations.Clear();
        Educations.Add(educationDatabaseConst.EDUCATION_0);
        Educations.Add(educationDatabaseConst.EDUCATION_1);
    }

    public void SetEducations(List<BaseDatabaseModel<EducationDataDatabaseModel>> educations)
    {
        Educations.Clear();
        Educations.AddRange(educations);
    }

    public List<BaseDatabaseModel<EducationDataDatabaseModel>> GetEducations()
    {
        return Educations;
    }
}
