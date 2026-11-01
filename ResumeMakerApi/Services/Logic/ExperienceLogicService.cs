using ResumeMakerApi.Models.Database.Base;
using ResumeMakerApi.Models.Database.Data;
using ResumeMakerApi.Models.Logic;
using ResumeMakerApi.Services.Database;

namespace ResumeMakerApi.Services.Logic;

public class ExperienceLogicService(ExperienceDatabaseService service)
{
    private int NextId = service.GetExperiences().Count;
    public ResponseLogicModel<BaseDatabaseModel<ExperienceDataDatabaseModel>> Select(int id)
    {
        var experienceToSelect = service.ReadById(id);
        if (experienceToSelect == null)
        {
            return new ResponseLogicModel<BaseDatabaseModel<ExperienceDataDatabaseModel>>
            {
                Success = false,
                Message = "Failed to select the experience because it does not exist.",
                Body = null
            };
        }
        var parentId = experienceToSelect.Metadata.ParentId;
        var experiences = parentId.HasValue
            ? service.ReadByParentId(parentId.Value)
            : service.ReadAll();
        foreach (var experience in experiences.ToList())
        {
            if (experience.Metadata.Id == experienceToSelect.Metadata.Id)
            {
                experience.Metadata.IsSelected = true;
            }
            else
            {
                experience.Metadata.IsSelected = false;
            }
            service.Update(experience);
        }
        var currentExperience = service.ReadById(id);
        return new ResponseLogicModel<BaseDatabaseModel<ExperienceDataDatabaseModel>>
        {
            Success = true,
            Message = "The experience was selected correctly.",
            Body = currentExperience
        };
    }
    public ResponseLogicModel<BaseDatabaseModel<ExperienceDataDatabaseModel>> Deselect(int id)
    {
        var experienceToDeselect = service.ReadById(id);
        if (experienceToDeselect == null)
        {
            return new ResponseLogicModel<BaseDatabaseModel<ExperienceDataDatabaseModel>>
            {
                Success = false,
                Message = "Failed to deselect the experience because it does not exist.",
                Body = null
            };
        }
        experienceToDeselect.Metadata.IsSelected = false;
        service.Update(experienceToDeselect);
        return new ResponseLogicModel<BaseDatabaseModel<ExperienceDataDatabaseModel>>
        {
            Success = true,
            Message = "The experience was deselected correctly.",
            Body = experienceToDeselect
        };
    }
    public ResponseLogicModel<BaseDatabaseModel<ExperienceDataDatabaseModel>> Create(ExperienceDataDatabaseModel data, int? parentId)
    {
        var experiences = parentId.HasValue
            ? service.ReadByParentId(parentId.Value)
            : service.ReadAll();
        var lastOrder = experiences.Count == 0 ? 0 : experiences.Max(experience => experience.Metadata.Order);
        var experienceToCreate = new BaseDatabaseModel<ExperienceDataDatabaseModel>
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
        service.Create(experienceToCreate);
        return new ResponseLogicModel<BaseDatabaseModel<ExperienceDataDatabaseModel>>
        {
            Success = true,
            Message = "The experience was created correctly.",
            Body = experienceToCreate
        };
    }
    public ResponseLogicModel<BaseDatabaseModel<ExperienceDataDatabaseModel>> ReadById(int id)
    {
        var experience = service.ReadById(id);
        if (experience == null)
        {
            return new ResponseLogicModel<BaseDatabaseModel<ExperienceDataDatabaseModel>>
            {
                Success = false,
                Message = "Failed to read the experience.",
                Body = null
            };
        }
        return new ResponseLogicModel<BaseDatabaseModel<ExperienceDataDatabaseModel>>
        {
            Success = true,
            Message = "The experience was read correctly.",
            Body = experience
        };
    }
    public ResponseLogicModel<List<BaseDatabaseModel<ExperienceDataDatabaseModel>>> ReadByParentId(int? parentId)
    {
        var experiences = parentId.HasValue
            ? service.ReadByParentId(parentId.Value)
            : service.ReadAll();
        if (experiences.Count == 0)
        {
            return new ResponseLogicModel<List<BaseDatabaseModel<ExperienceDataDatabaseModel>>>
            {
                Success = false,
                Message = "The list of experiences is empty.",
                Body = []
            };
        }
        return new ResponseLogicModel<List<BaseDatabaseModel<ExperienceDataDatabaseModel>>>
        {
            Success = true,
            Message = "The experiences were read correctly.",
            Body = experiences
        };
    }
    public ResponseLogicModel<BaseDatabaseModel<ExperienceDataDatabaseModel>> Update(int id, ExperienceDataDatabaseModel data)
    {
        var experienceToUpdate = service.ReadById(id);
        if (experienceToUpdate == null)
        {
            return new ResponseLogicModel<BaseDatabaseModel<ExperienceDataDatabaseModel>>
            {
                Success = false,
                Message = "Failed to update the experience because it does not exist.",
                Body = null
            };
        }
        experienceToUpdate.Data = data;
        service.Update(experienceToUpdate);
        return new ResponseLogicModel<BaseDatabaseModel<ExperienceDataDatabaseModel>>
        {
            Success = true,
            Message = "The experience was updated correctly.",
            Body = experienceToUpdate
        };
    }
    public ResponseLogicModel<BaseDatabaseModel<ExperienceDataDatabaseModel>> Delete(int id)
    {
        var experienceToDelete = service.ReadById(id);
        if (experienceToDelete == null)
        {
            return new ResponseLogicModel<BaseDatabaseModel<ExperienceDataDatabaseModel>>
            {
                Success = false,
                Message = "Failed to delete the experience because it does not exist.",
                Body = null
            };
        }
        var parentId = experienceToDelete.Metadata.ParentId;
        service.Delete(experienceToDelete.Metadata.Id);
        var experiencesToUpdate = parentId.HasValue
            ? service.ReadByParentId(parentId.Value)
            : service.ReadAll();
        experiencesToUpdate
            .OrderBy(item => item.Metadata.Order)
            .Select((itemToUpdate, index) =>
            {
                itemToUpdate.Metadata.Order = index;
                return itemToUpdate;
            })
            .ToList()
            .ForEach(service.Update);
        return new ResponseLogicModel<BaseDatabaseModel<ExperienceDataDatabaseModel>>
        {
            Success = true,
            Message = "The experience was deleted correctly.",
            Body = experienceToDelete
        };
    }
    public ResponseLogicModel<BaseDatabaseModel<ExperienceDataDatabaseModel>> MoveUp(int id)
    {
        var experienceToMoveUp = service.ReadById(id);
        if (experienceToMoveUp == null)
        {
            return new ResponseLogicModel<BaseDatabaseModel<ExperienceDataDatabaseModel>>
            {
                Success = false,
                Message = "Failed to move up the experience because it does not exist.",
                Body = null
            };
        }
        if (experienceToMoveUp.Metadata.Order == 0)
        {
            return new ResponseLogicModel<BaseDatabaseModel<ExperienceDataDatabaseModel>>
            {
                Success = false,
                Message = "Failed to move up the experience because it is the first element.",
                Body = null
            };
        }
        var experiences = experienceToMoveUp.Metadata.ParentId.HasValue
            ? service.ReadByParentId(experienceToMoveUp.Metadata.ParentId.Value)
            : service.ReadAll();
        var prevExperience = experiences.FirstOrDefault(
            item => item.Metadata.Order == experienceToMoveUp.Metadata.Order - 1
        );
        if (prevExperience == null)
        {
            return new ResponseLogicModel<BaseDatabaseModel<ExperienceDataDatabaseModel>>
            {
                Success = false,
                Message = "Failed to move up the experience because the previous experience does not exist.",
                Body = null
            };
        }
        var experienceToMoveUpOrder = experienceToMoveUp.Metadata.Order;
        experienceToMoveUp.Metadata.Order = prevExperience.Metadata.Order;
        prevExperience.Metadata.Order = experienceToMoveUpOrder;
        service.Update(experienceToMoveUp);
        service.Update(prevExperience);
        return new ResponseLogicModel<BaseDatabaseModel<ExperienceDataDatabaseModel>>
        {
            Success = true,
            Message = "The experience was moved up correctly.",
            Body = experienceToMoveUp
        };
    }
    public ResponseLogicModel<BaseDatabaseModel<ExperienceDataDatabaseModel>> MoveDown(int id)
    {
        var experienceToMoveDown = service.ReadById(id);
        if (experienceToMoveDown == null)
        {
            return new ResponseLogicModel<BaseDatabaseModel<ExperienceDataDatabaseModel>>
            {
                Success = false,
                Message = "Failed to move down the experience because it does not exist.",
                Body = null
            };
        }
        var experiences = experienceToMoveDown.Metadata.ParentId.HasValue
            ? service.ReadByParentId(experienceToMoveDown.Metadata.ParentId.Value)
            : service.ReadAll();
        var lastOrder = experiences.Aggregate(
            0,
            (acc, curr) => curr.Metadata.Order > acc
                ? curr.Metadata.Order
                : acc
        );
        if (experienceToMoveDown.Metadata.Order == lastOrder)
        {
            return new ResponseLogicModel<BaseDatabaseModel<ExperienceDataDatabaseModel>>
            {
                Success = false,
                Message = "Failed to move down the experience because it is the last element.",
                Body = null
            };
        }
        var nextExperience = experiences.FirstOrDefault(
            experience => experience.Metadata.Order == experienceToMoveDown.Metadata.Order + 1
        );
        if (nextExperience == null)
        {
            return new ResponseLogicModel<BaseDatabaseModel<ExperienceDataDatabaseModel>>
            {
                Success = false,
                Message = "Failed to move down the experience because the next experience does not exist.",
                Body = null
            };
        }
        var experienceToMoveDownOrder = experienceToMoveDown.Metadata.Order;
        experienceToMoveDown.Metadata.Order = nextExperience.Metadata.Order;
        nextExperience.Metadata.Order = experienceToMoveDownOrder;
        service.Update(experienceToMoveDown);
        service.Update(nextExperience);
        return new ResponseLogicModel<BaseDatabaseModel<ExperienceDataDatabaseModel>>
        {
            Success = true,
            Message = "The experience was moved down correctly.",
            Body = experienceToMoveDown
        };
    }
}