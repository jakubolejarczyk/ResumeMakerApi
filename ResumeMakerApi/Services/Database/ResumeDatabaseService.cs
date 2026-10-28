using ResumeMakerApi.Data.Database;
using ResumeMakerApi.Models.Database.Base;
using ResumeMakerApi.Models.Database.Data;

namespace ResumeMakerApi.Services.Database;

public class ResumeDatabaseService(ResumeDatabaseData resumeDatabaseData)
{
    public void Create(BaseDatabaseModel<ResumeDataDatabaseModel> resumeToCreate)
    {
        var resumes = ReadAll();
        resumes.Add(resumeToCreate);
        resumeDatabaseData.SetResumes(resumes);
    }

    public List<BaseDatabaseModel<ResumeDataDatabaseModel>> ReadAll()
    {
        return resumeDatabaseData.GetResumes();
    }

    public BaseDatabaseModel<ResumeDataDatabaseModel>? ReadById(int id)
    {
        return ReadAll().Find(resume => resume.Metadata.Id == id);
    }

    public List<BaseDatabaseModel<ResumeDataDatabaseModel>> ReadByParentId(int parentId)
    {
        return [..ReadAll().Where(resume =>
        {
            if (resume.Metadata.ParentId == null)
            {
                return false;
            }
            return resume.Metadata.ParentId == parentId;
        })];
    }

    public void Update(BaseDatabaseModel<ResumeDataDatabaseModel> resumeToUpdate)
    {
        var resumes = ReadAll().Select((resume) =>
        {
            if (resume.Metadata.Id == resumeToUpdate.Metadata.Id)
            {
                return resumeToUpdate;
            }
            return resume;
        }).ToList();
        resumeDatabaseData.SetResumes(resumes);
    }

    public void Delete(int id)
    {
        var resumes = resumeDatabaseData.GetResumes()
            .Where(resume => resume.Metadata.Id != id)
            .ToList();
        resumeDatabaseData.SetResumes(resumes);
    }

    public List<BaseDatabaseModel<ResumeDataDatabaseModel>> GetResumes()
    {
        return resumeDatabaseData.GetResumes();
    }
}
