using ResumeMakerApi.Models.Database.Base;
using ResumeMakerApi.Models.Database.Data;
using ResumeMakerApi.Models.Logic;
using ResumeMakerApi.Services.Database;

namespace ResumeMakerApi.Services.Logic;

public class ResumeLogicService(ResumeDatabaseService service)
{
    private int NextId = service.GetResumes().Count;
    public ResponseLogicModel<BaseDatabaseModel<ResumeDataDatabaseModel>> Select(int id)
    {
        var resumeToSelect = service.ReadById(id);
        if (resumeToSelect == null)
        {
            return new ResponseLogicModel<BaseDatabaseModel<ResumeDataDatabaseModel>>
            {
                Success = false,
                Message = "Failed to select the resume because it does not exist.",
                Body = null
            };
        }
        var parentId = resumeToSelect.Metadata.ParentId;
        var resumes = parentId.HasValue
            ? service.ReadByParentId(parentId.Value)
            : service.ReadAll();
        foreach (var resume in resumes)
        {
            if (resume.Metadata.Id == resumeToSelect.Metadata.Id)
            {
                resume.Metadata.IsSelected = true;
            }
            else
            {
                resume.Metadata.IsSelected = false;
            }
            service.Update(resume);
        }
        var currentResume = service.ReadById(id);
        return new ResponseLogicModel<BaseDatabaseModel<ResumeDataDatabaseModel>>
        {
            Success = true,
            Message = "The resume was selected correctly.",
            Body = currentResume
        };
    }
    public ResponseLogicModel<BaseDatabaseModel<ResumeDataDatabaseModel>> Deselect(int id)
    {
        var resumeToDeselect = service.ReadById(id);
        if (resumeToDeselect == null)
        {
            return new ResponseLogicModel<BaseDatabaseModel<ResumeDataDatabaseModel>>
            {
                Success = false,
                Message = "Failed to deselect the resume because it does not exist.",
                Body = null
            };
        }
        resumeToDeselect.Metadata.IsSelected = false;
        service.Update(resumeToDeselect);
        return new ResponseLogicModel<BaseDatabaseModel<ResumeDataDatabaseModel>>
        {
            Success = true,
            Message = "The resume was deselected correctly.",
            Body = resumeToDeselect
        };
    }
    public ResponseLogicModel<BaseDatabaseModel<ResumeDataDatabaseModel>> Create(ResumeDataDatabaseModel data, int? parentId)
    {
        var resumes = parentId.HasValue
            ? service.ReadByParentId(parentId.Value)
            : service.ReadAll();
        var lastOrder = resumes.Max(resume => resume.Metadata.Order);
        var resumeToCreate = new BaseDatabaseModel<ResumeDataDatabaseModel>
        {
            Data = data,
            Metadata = new BaseDatabaseMetadataModel
            {
                Id = NextId,
                ParentId = parentId,
                Order = lastOrder + 1,
                IsSelected = false
            }
        };
        NextId++;
        service.Create(resumeToCreate);
        return new ResponseLogicModel<BaseDatabaseModel<ResumeDataDatabaseModel>>
        {
            Success = true,
            Message = "The resume was created correctly.",
            Body = resumeToCreate
        };
    }
    public ResponseLogicModel<BaseDatabaseModel<ResumeDataDatabaseModel>> ReadById(int id)
    {
        var resume = service.ReadById(id);
        if (resume == null)
        {
            return new ResponseLogicModel<BaseDatabaseModel<ResumeDataDatabaseModel>>
            {
                Success = false,
                Message = "Failed to read the resume.",
                Body = null
            };
        }
        return new ResponseLogicModel<BaseDatabaseModel<ResumeDataDatabaseModel>>
        {
            Success = true,
            Message = "The resume was read correctly.",
            Body = resume
        };
    }
    public ResponseLogicModel<List<BaseDatabaseModel<ResumeDataDatabaseModel>>> ReadByParentId(int? parentId)
    {
        var resumes = parentId.HasValue
            ? service.ReadByParentId(parentId.Value)
            : service.ReadAll();
        if (resumes.Count == 0)
        {
            return new ResponseLogicModel<List<BaseDatabaseModel<ResumeDataDatabaseModel>>>
            {
                Success = false,
                Message = "The list of resumes is empty.",
                Body = []
            };
        }
        return new ResponseLogicModel<List<BaseDatabaseModel<ResumeDataDatabaseModel>>>
        {
            Success = true,
            Message = "The resumes were read correctly.",
            Body = resumes
        };
    }
    public ResponseLogicModel<BaseDatabaseModel<ResumeDataDatabaseModel>> Update(int id, ResumeDataDatabaseModel data)
    {
        var resumeToUpdate = service.ReadById(id);
        if (resumeToUpdate == null)
        {
            return new ResponseLogicModel<BaseDatabaseModel<ResumeDataDatabaseModel>>
            {
                Success = false,
                Message = "Failed to update the resume because it does not exist.",
                Body = null
            };
        }
        resumeToUpdate.Data = data;
        service.Update(resumeToUpdate);
        return new ResponseLogicModel<BaseDatabaseModel<ResumeDataDatabaseModel>>
        {
            Success = true,
            Message = "The resume was updated correctly.",
            Body = resumeToUpdate
        };
    }
    public ResponseLogicModel<BaseDatabaseModel<ResumeDataDatabaseModel>> Delete(int id)
    {
        var resumeToDelete = service.ReadById(id);
        if (resumeToDelete == null)
        {
            return new ResponseLogicModel<BaseDatabaseModel<ResumeDataDatabaseModel>>
            {
                Success = false,
                Message = "Failed to delete the resume because it does not exist.",
                Body = null
            };
        }
        var parentId = resumeToDelete.Metadata.ParentId;
        service.Delete(resumeToDelete.Metadata.Id);
        var resumesToUpdate = parentId.HasValue
            ? service.ReadByParentId(parentId.Value)
            : service.ReadAll();
        resumesToUpdate
            .OrderBy(item => item.Metadata.Order)
            .Select((itemToUpdate, index) =>
            {
                itemToUpdate.Metadata.Order = index;
                return itemToUpdate;
            })
            .ToList()
            .ForEach(service.Update);
        return new ResponseLogicModel<BaseDatabaseModel<ResumeDataDatabaseModel>>
        {
            Success = true,
            Message = "The resume was deleted correctly.",
            Body = resumeToDelete
        };
    }
    public ResponseLogicModel<BaseDatabaseModel<ResumeDataDatabaseModel>> MoveUp(int id)
    {
        var resumeToMoveUp = service.ReadById(id);
        if (resumeToMoveUp == null)
        {
            return new ResponseLogicModel<BaseDatabaseModel<ResumeDataDatabaseModel>>
            {
                Success = false,
                Message = "Failed to move up the resume because it does not exist.",
                Body = null
            };
        }
        if (resumeToMoveUp.Metadata.Order == 0)
        {
            return new ResponseLogicModel<BaseDatabaseModel<ResumeDataDatabaseModel>>
            {
                Success = false,
                Message = "Failed to move up the resume because it is the first element.",
                Body = null
            };
        }
        var resumes = resumeToMoveUp.Metadata.ParentId.HasValue
            ? service.ReadByParentId(resumeToMoveUp.Metadata.ParentId.Value)
            : service.ReadAll();
        var prevResume = resumes.FirstOrDefault(
            item => item.Metadata.Order == resumeToMoveUp.Metadata.Order - 1
        );
        if (prevResume == null)
        {
            return new ResponseLogicModel<BaseDatabaseModel<ResumeDataDatabaseModel>>
            {
                Success = false,
                Message = "Failed to move up the resume because the previous resume does not exist.",
                Body = null
            };
        }
        var resumeToMoveUpOrder = resumeToMoveUp.Metadata.Order;
        resumeToMoveUp.Metadata.Order = prevResume.Metadata.Order;
        prevResume.Metadata.Order = resumeToMoveUpOrder;
        service.Update(resumeToMoveUp);
        service.Update(prevResume);
        return new ResponseLogicModel<BaseDatabaseModel<ResumeDataDatabaseModel>>
        {
            Success = true,
            Message = "The resume was moved up correctly.",
            Body = resumeToMoveUp
        };
    }
    public ResponseLogicModel<BaseDatabaseModel<ResumeDataDatabaseModel>> MoveDown(int id)
    {
        var resumeToMoveDown = service.ReadById(id);
        if (resumeToMoveDown == null)
        {
            return new ResponseLogicModel<BaseDatabaseModel<ResumeDataDatabaseModel>>
            {
                Success = false,
                Message = "Failed to move down the resume because it does not exist.",
                Body = null
            };
        }
        var resumes = resumeToMoveDown.Metadata.ParentId.HasValue
            ? service.ReadByParentId(resumeToMoveDown.Metadata.ParentId.Value)
            : service.ReadAll();
        var lastOrder = resumes.Aggregate(
            0,
            (acc, curr) => curr.Metadata.Order > acc
                ? curr.Metadata.Order
                : acc
        );
        if (resumeToMoveDown.Metadata.Order == lastOrder)
        {
            return new ResponseLogicModel<BaseDatabaseModel<ResumeDataDatabaseModel>>
            {
                Success = false,
                Message = "Failed to move down the resume because it is the last element.",
                Body = null
            };
        }
        var nextResume = resumes.FirstOrDefault(
            resume => resume.Metadata.Order == resumeToMoveDown.Metadata.Order + 1
        );
        if (nextResume == null)
        {
            return new ResponseLogicModel<BaseDatabaseModel<ResumeDataDatabaseModel>>
            {
                Success = false,
                Message = "Failed to move down the resume because the next resume does not exist.",
                Body = null
            };
        }
        var resumeToMoveDownOrder = resumeToMoveDown.Metadata.Order;
        resumeToMoveDown.Metadata.Order = nextResume.Metadata.Order;
        nextResume.Metadata.Order = resumeToMoveDownOrder;
        service.Update(resumeToMoveDown);
        service.Update(nextResume);
        return new ResponseLogicModel<BaseDatabaseModel<ResumeDataDatabaseModel>>
        {
            Success = true,
            Message = "The resume was moved down correctly.",
            Body = resumeToMoveDown
        };
    }
}