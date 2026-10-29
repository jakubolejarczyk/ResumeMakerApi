using ResumeMakerApi.Data.Database;
using ResumeMakerApi.Models.Database.Base;
using ResumeMakerApi.Models.Database.Data;

namespace ResumeMakerApi.Services.Database;

public class EducationDatabaseService(EducationDatabaseData educationDatabaseData)
{
    public void Create(BaseDatabaseModel<EducationDataDatabaseModel> educationToCreate)
    {
        var educations = ReadAll();
        educations.Add(educationToCreate);
        educationDatabaseData.SetEducations(educations);
    }

    public List<BaseDatabaseModel<EducationDataDatabaseModel>> ReadAll()
    {
        return educationDatabaseData.GetEducations();
    }

    public BaseDatabaseModel<EducationDataDatabaseModel>? ReadById(int id)
    {
        return ReadAll().Find(education => education.Metadata.Id == id);
    }

    public List<BaseDatabaseModel<EducationDataDatabaseModel>> ReadByParentId(int parentId)
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

    public void Update(BaseDatabaseModel<EducationDataDatabaseModel> educationToUpdate)
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

    public List<BaseDatabaseModel<EducationDataDatabaseModel>> GetEducations()
    {
        return educationDatabaseData.GetEducations();
    }
}
