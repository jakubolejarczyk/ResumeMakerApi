using ResumeMakerApi.Data.Database;
using ResumeMakerApi.Models.Database.Base;
using ResumeMakerApi.Models.Database.Data;

namespace ResumeMakerApi.Services.Database;

public class EducationDatabaseService(EducationDatabaseData educationDatabaseData)
{
    public void Create(BaseDatabaseModel<EducationDatabaseModel> educationToCreate)
    {
        var educations = ReadAll();
        educations.Add(educationToCreate);
        educationDatabaseData.SetEducations(educations);
    }

    public List<BaseDatabaseModel<EducationDatabaseModel>> ReadAll()
    {
        return educationDatabaseData.GetEducations();
    }

    public BaseDatabaseModel<EducationDatabaseModel>? ReadById(int id)
    {
        return ReadAll().Find(education => education.Metadata.Id == id);
    }

    public List<BaseDatabaseModel<EducationDatabaseModel>> ReadByParentId(int parentId)
    {
        return [..ReadAll().Where(education =>
        {
            if (education.Metadata.ParentId == null)
            {
                return false;
            }
            return education.Metadata.ParentId == parentId;
        })];
    }

    public void Update(BaseDatabaseModel<EducationDatabaseModel> educationToUpdate)
    {
        var educations = ReadAll().Select((education) =>
        {
            if (education.Metadata.Id == educationToUpdate.Metadata.Id)
            {
                return educationToUpdate;
            }
            return education;
        }).ToList();
        educationDatabaseData.SetEducations(educations);
    }

    public void Delete(int id)
    {
        var educations = educationDatabaseData.GetEducations()
            .Where(education => education.Metadata.Id != id)
            .ToList();
        educationDatabaseData.SetEducations(educations);
    }

    public List<BaseDatabaseModel<EducationDatabaseModel>> GetEducations()
    {
        return educationDatabaseData.GetEducations();
    }
}
