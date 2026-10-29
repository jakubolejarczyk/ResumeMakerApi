using ResumeMakerApi.Models.Database.Base;
using ResumeMakerApi.Models.Database.Data;
using ResumeMakerApi.Models.Logic;
using ResumeMakerApi.Services.Database;

namespace ResumeMakerApi.Services.Logic;

public class EducationLogicService(EducationDatabaseService service)
{
    private int NextId = service.GetEducations().Count;
    public ResponseLogicModel<BaseDatabaseModel<EducationDataDatabaseModel>> Select(int id)
    {
        var educationToSelect = service.ReadById(id);
        if (educationToSelect == null)
        {
            return new ResponseLogicModel<BaseDatabaseModel<EducationDataDatabaseModel>>
            {
                Success = false,
                Message = "Failed to select the education because it does not exist.",
                Body = null
            };
        }
        var parentId = educationToSelect.Metadata.ParentId;
        var educations = parentId.HasValue
            ? service.ReadByParentId(parentId.Value)
            : service.ReadAll();
        foreach (var education in educations)
        {
            if (education.Metadata.Id == educationToSelect.Metadata.Id)
            {
                education.Metadata.IsSelected = true;
            }
            else
            {
                education.Metadata.IsSelected = false;
            }
            service.Update(education);
        }
        var currentEducation = service.ReadById(id);
        return new ResponseLogicModel<BaseDatabaseModel<EducationDataDatabaseModel>>
        {
            Success = true,
            Message = "The education was selected correctly.",
            Body = currentEducation
        };
    }
    public ResponseLogicModel<BaseDatabaseModel<EducationDataDatabaseModel>> Deselect(int id)
    {
        var educationToDeselect = service.ReadById(id);
        if (educationToDeselect == null)
        {
            return new ResponseLogicModel<BaseDatabaseModel<EducationDataDatabaseModel>>
            {
                Success = false,
                Message = "Failed to deselect the education because it does not exist.",
                Body = null
            };
        }
        educationToDeselect.Metadata.IsSelected = false;
        service.Update(educationToDeselect);
        return new ResponseLogicModel<BaseDatabaseModel<EducationDataDatabaseModel>>
        {
            Success = true,
            Message = "The education was deselected correctly.",
            Body = educationToDeselect
        };
    }
    public ResponseLogicModel<BaseDatabaseModel<EducationDataDatabaseModel>> Create(EducationDataDatabaseModel data, int? parentId)
    {
        var educations = parentId.HasValue
            ? service.ReadByParentId(parentId.Value)
            : service.ReadAll();
        var lastOrder = educations.Max(education => education.Metadata.Order);
        var educationToCreate = new BaseDatabaseModel<EducationDataDatabaseModel>
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
        service.Create(educationToCreate);
        return new ResponseLogicModel<BaseDatabaseModel<EducationDataDatabaseModel>>
        {
            Success = true,
            Message = "The education was created correctly.",
            Body = educationToCreate
        };
    }
    public ResponseLogicModel<BaseDatabaseModel<EducationDataDatabaseModel>> ReadById(int id)
    {
        var education = service.ReadById(id);
        if (education == null)
        {
            return new ResponseLogicModel<BaseDatabaseModel<EducationDataDatabaseModel>>
            {
                Success = false,
                Message = "Failed to read the education.",
                Body = null
            };
        }
        return new ResponseLogicModel<BaseDatabaseModel<EducationDataDatabaseModel>>
        {
            Success = true,
            Message = "The education was read correctly.",
            Body = education
        };
    }
    public ResponseLogicModel<List<BaseDatabaseModel<EducationDataDatabaseModel>>> ReadByParentId(int? parentId)
    {
        var educations = parentId.HasValue
            ? service.ReadByParentId(parentId.Value)
            : service.ReadAll();
        if (educations.Count == 0)
        {
            return new ResponseLogicModel<List<BaseDatabaseModel<EducationDataDatabaseModel>>>
            {
                Success = false,
                Message = "The list of educations is empty.",
                Body = []
            };
        }
        return new ResponseLogicModel<List<BaseDatabaseModel<EducationDataDatabaseModel>>>
        {
            Success = true,
            Message = "The educations were read correctly.",
            Body = educations
        };
    }
    public ResponseLogicModel<BaseDatabaseModel<EducationDataDatabaseModel>> Update(int id, EducationDataDatabaseModel data)
    {
        var educationToUpdate = service.ReadById(id);
        if (educationToUpdate == null)
        {
            return new ResponseLogicModel<BaseDatabaseModel<EducationDataDatabaseModel>>
            {
                Success = false,
                Message = "Failed to update the education because it does not exist.",
                Body = null
            };
        }
        educationToUpdate.Data = data;
        service.Update(educationToUpdate);
        return new ResponseLogicModel<BaseDatabaseModel<EducationDataDatabaseModel>>
        {
            Success = true,
            Message = "The education was updated correctly.",
            Body = educationToUpdate
        };
    }
    public ResponseLogicModel<BaseDatabaseModel<EducationDataDatabaseModel>> Delete(int id)
    {
        var educationToDelete = service.ReadById(id);
        if (educationToDelete == null)
        {
            return new ResponseLogicModel<BaseDatabaseModel<EducationDataDatabaseModel>>
            {
                Success = false,
                Message = "Failed to delete the education because it does not exist.",
                Body = null
            };
        }
        var parentId = educationToDelete.Metadata.ParentId;
        service.Delete(educationToDelete.Metadata.Id);
        var educationsToUpdate = parentId.HasValue
            ? service.ReadByParentId(parentId.Value)
            : service.ReadAll();
        educationsToUpdate
            .OrderBy(item => item.Metadata.Order)
            .Select((itemToUpdate, index) =>
            {
                itemToUpdate.Metadata.Order = index;
                return itemToUpdate;
            })
            .ToList()
            .ForEach(service.Update);
        return new ResponseLogicModel<BaseDatabaseModel<EducationDataDatabaseModel>>
        {
            Success = true,
            Message = "The education was deleted correctly.",
            Body = educationToDelete
        };
    }
    public ResponseLogicModel<BaseDatabaseModel<EducationDataDatabaseModel>> MoveUp(int id)
    {
        var educationToMoveUp = service.ReadById(id);
        if (educationToMoveUp == null)
        {
            return new ResponseLogicModel<BaseDatabaseModel<EducationDataDatabaseModel>>
            {
                Success = false,
                Message = "Failed to move up the education because it does not exist.",
                Body = null
            };
        }
        if (educationToMoveUp.Metadata.Order == 0)
        {
            return new ResponseLogicModel<BaseDatabaseModel<EducationDataDatabaseModel>>
            {
                Success = false,
                Message = "Failed to move up the education because it is the first element.",
                Body = null
            };
        }
        var educations = educationToMoveUp.Metadata.ParentId.HasValue
            ? service.ReadByParentId(educationToMoveUp.Metadata.ParentId.Value)
            : service.ReadAll();
        var prevEducation = educations.FirstOrDefault(
            item => item.Metadata.Order == educationToMoveUp.Metadata.Order - 1
        );
        if (prevEducation == null)
        {
            return new ResponseLogicModel<BaseDatabaseModel<EducationDataDatabaseModel>>
            {
                Success = false,
                Message = "Failed to move up the education because the previous education does not exist.",
                Body = null
            };
        }
        var educationToMoveUpOrder = educationToMoveUp.Metadata.Order;
        educationToMoveUp.Metadata.Order = prevEducation.Metadata.Order;
        prevEducation.Metadata.Order = educationToMoveUpOrder;
        service.Update(educationToMoveUp);
        service.Update(prevEducation);
        return new ResponseLogicModel<BaseDatabaseModel<EducationDataDatabaseModel>>
        {
            Success = true,
            Message = "The education was moved up correctly.",
            Body = educationToMoveUp
        };
    }
    public ResponseLogicModel<BaseDatabaseModel<EducationDataDatabaseModel>> MoveDown(int id)
    {
        var educationToMoveDown = service.ReadById(id);
        if (educationToMoveDown == null)
        {
            return new ResponseLogicModel<BaseDatabaseModel<EducationDataDatabaseModel>>
            {
                Success = false,
                Message = "Failed to move down the education because it does not exist.",
                Body = null
            };
        }
        var educations = educationToMoveDown.Metadata.ParentId.HasValue
            ? service.ReadByParentId(educationToMoveDown.Metadata.ParentId.Value)
            : service.ReadAll();
        var lastOrder = educations.Aggregate(
            0,
            (acc, curr) => curr.Metadata.Order > acc
                ? curr.Metadata.Order
                : acc
        );
        if (educationToMoveDown.Metadata.Order == lastOrder)
        {
            return new ResponseLogicModel<BaseDatabaseModel<EducationDataDatabaseModel>>
            {
                Success = false,
                Message = "Failed to move down the education because it is the last element.",
                Body = null
            };
        }
        var nextEducation = educations.FirstOrDefault(
            education => education.Metadata.Order == educationToMoveDown.Metadata.Order + 1
        );
        if (nextEducation == null)
        {
            return new ResponseLogicModel<BaseDatabaseModel<EducationDataDatabaseModel>>
            {
                Success = false,
                Message = "Failed to move down the education because the next education does not exist.",
                Body = null
            };
        }
        var educationToMoveDownOrder = educationToMoveDown.Metadata.Order;
        educationToMoveDown.Metadata.Order = nextEducation.Metadata.Order;
        nextEducation.Metadata.Order = educationToMoveDownOrder;
        service.Update(educationToMoveDown);
        service.Update(nextEducation);
        return new ResponseLogicModel<BaseDatabaseModel<EducationDataDatabaseModel>>
        {
            Success = true,
            Message = "The education was moved down correctly.",
            Body = educationToMoveDown
        };
    }
}