using ResumeMakerApi.Consts.Database;
using ResumeMakerApi.Models.Database.Base;
using ResumeMakerApi.Models.Database.Data;

namespace ResumeMakerApi.Data.Database;

public class ResumeDatabaseData
{
    private readonly List<BaseDatabaseModel<ResumeDataDatabaseModel>> Resumes = [];

    public ResumeDatabaseData(ResumeDatabaseConst resumeDatabaseConst)
    {
        Resumes.Clear();
        Resumes.Add(resumeDatabaseConst.RESUME_0);
    }

    public void SetResumes(List<BaseDatabaseModel<ResumeDataDatabaseModel>> resumes)
    {
        Resumes.Clear();
        Resumes.AddRange(resumes);
    }

    public List<BaseDatabaseModel<ResumeDataDatabaseModel>> GetResumes()
    {
        return Resumes;
    }
}
