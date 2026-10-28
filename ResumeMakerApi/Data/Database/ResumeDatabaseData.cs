using ResumeMakerApi.Consts.Database;
using ResumeMakerApi.Models.Database.Base;
using ResumeMakerApi.Models.Database.Data;

namespace ResumeMakerApi.Data.Database;

public class ResumeDatabaseData(ResumeDatabaseConst resumeDatabaseConst)
{
    private readonly List<BaseDatabaseModel<ResumeDataDatabaseModel>> Resumes = [];

    public void Init()
    {
        Resumes.Clear();
        Resumes.Add(resumeDatabaseConst.RESUME_0);
    }

    public void Set(List<BaseDatabaseModel<ResumeDataDatabaseModel>> resumes)
    {
        Resumes.Clear();
        Resumes.AddRange(resumes);
    }

    public List<BaseDatabaseModel<ResumeDataDatabaseModel>> Get()
    {
        return Resumes;
    }
}
