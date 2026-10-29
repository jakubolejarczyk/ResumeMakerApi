using ResumeMakerApi.Models.Database.Base;
using ResumeMakerApi.Models.Database.Data;
using ResumeMakerApi.Models.Logic;
using ResumeMakerApi.Services.Database;

namespace ResumeMakerApi.Services.Logic;

public class SkillLogicService(SkillDatabaseService service)
{
    private int NextId = service.GetSkills().Count;
    public ResponseLogicModel<BaseDatabaseModel<SkillDataDatabaseModel>> Select(int id)
    {
        var skillToSelect = service.ReadById(id);
        if (skillToSelect == null)
        {
            return new ResponseLogicModel<BaseDatabaseModel<SkillDataDatabaseModel>>
            {
                Success = false,
                Message = "Failed to select the skill because it does not exist.",
                Body = null
            };
        }
        var parentId = skillToSelect.Metadata.ParentId;
        var skills = parentId.HasValue
            ? service.ReadByParentId(parentId.Value)
            : service.ReadAll();
        foreach (var skill in skills)
        {
            if (skill.Metadata.Id == skillToSelect.Metadata.Id)
            {
                skill.Metadata.IsSelected = true;
            }
            else
            {
                skill.Metadata.IsSelected = false;
            }
            service.Update(skill);
        }
        var currentSkill = service.ReadById(id);
        return new ResponseLogicModel<BaseDatabaseModel<SkillDataDatabaseModel>>
        {
            Success = true,
            Message = "The skill was selected correctly.",
            Body = currentSkill
        };
    }
    public ResponseLogicModel<BaseDatabaseModel<SkillDataDatabaseModel>> Deselect(int id)
    {
        var skillToDeselect = service.ReadById(id);
        if (skillToDeselect == null)
        {
            return new ResponseLogicModel<BaseDatabaseModel<SkillDataDatabaseModel>>
            {
                Success = false,
                Message = "Failed to deselect the skill because it does not exist.",
                Body = null
            };
        }
        skillToDeselect.Metadata.IsSelected = false;
        service.Update(skillToDeselect);
        return new ResponseLogicModel<BaseDatabaseModel<SkillDataDatabaseModel>>
        {
            Success = true,
            Message = "The skill was deselected correctly.",
            Body = skillToDeselect
        };
    }
    public ResponseLogicModel<BaseDatabaseModel<SkillDataDatabaseModel>> Create(SkillDataDatabaseModel data, int? parentId)
    {
        var skills = parentId.HasValue
            ? service.ReadByParentId(parentId.Value)
            : service.ReadAll();
        var lastOrder = skills.Max(skill => skill.Metadata.Order);
        var skillToCreate = new BaseDatabaseModel<SkillDataDatabaseModel>
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
        service.Create(skillToCreate);
        return new ResponseLogicModel<BaseDatabaseModel<SkillDataDatabaseModel>>
        {
            Success = true,
            Message = "The skill was created correctly.",
            Body = skillToCreate
        };
    }
    public ResponseLogicModel<BaseDatabaseModel<SkillDataDatabaseModel>> ReadById(int id)
    {
        var skill = service.ReadById(id);
        if (skill == null)
        {
            return new ResponseLogicModel<BaseDatabaseModel<SkillDataDatabaseModel>>
            {
                Success = false,
                Message = "Failed to read the skill.",
                Body = null
            };
        }
        return new ResponseLogicModel<BaseDatabaseModel<SkillDataDatabaseModel>>
        {
            Success = true,
            Message = "The skill was read correctly.",
            Body = skill
        };
    }
    public ResponseLogicModel<List<BaseDatabaseModel<SkillDataDatabaseModel>>> ReadByParentId(int? parentId)
    {
        var skills = parentId.HasValue
            ? service.ReadByParentId(parentId.Value)
            : service.ReadAll();
        if (skills.Count == 0)
        {
            return new ResponseLogicModel<List<BaseDatabaseModel<SkillDataDatabaseModel>>>
            {
                Success = false,
                Message = "The list of skills is empty.",
                Body = []
            };
        }
        return new ResponseLogicModel<List<BaseDatabaseModel<SkillDataDatabaseModel>>>
        {
            Success = true,
            Message = "The skills were read correctly.",
            Body = skills
        };
    }
    public ResponseLogicModel<BaseDatabaseModel<SkillDataDatabaseModel>> Update(int id, SkillDataDatabaseModel data)
    {
        var skillToUpdate = service.ReadById(id);
        if (skillToUpdate == null)
        {
            return new ResponseLogicModel<BaseDatabaseModel<SkillDataDatabaseModel>>
            {
                Success = false,
                Message = "Failed to update the skill because it does not exist.",
                Body = null
            };
        }
        skillToUpdate.Data = data;
        service.Update(skillToUpdate);
        return new ResponseLogicModel<BaseDatabaseModel<SkillDataDatabaseModel>>
        {
            Success = true,
            Message = "The skill was updated correctly.",
            Body = skillToUpdate
        };
    }
    public ResponseLogicModel<BaseDatabaseModel<SkillDataDatabaseModel>> Delete(int id)
    {
        var skillToDelete = service.ReadById(id);
        if (skillToDelete == null)
        {
            return new ResponseLogicModel<BaseDatabaseModel<SkillDataDatabaseModel>>
            {
                Success = false,
                Message = "Failed to delete the skill because it does not exist.",
                Body = null
            };
        }
        var parentId = skillToDelete.Metadata.ParentId;
        service.Delete(skillToDelete.Metadata.Id);
        var skillsToUpdate = parentId.HasValue
            ? service.ReadByParentId(parentId.Value)
            : service.ReadAll();
        skillsToUpdate
            .OrderBy(item => item.Metadata.Order)
            .Select((itemToUpdate, index) =>
            {
                itemToUpdate.Metadata.Order = index;
                return itemToUpdate;
            })
            .ToList()
            .ForEach(service.Update);
        return new ResponseLogicModel<BaseDatabaseModel<SkillDataDatabaseModel>>
        {
            Success = true,
            Message = "The skill was deleted correctly.",
            Body = skillToDelete
        };
    }
    public ResponseLogicModel<BaseDatabaseModel<SkillDataDatabaseModel>> MoveUp(int id)
    {
        var skillToMoveUp = service.ReadById(id);
        if (skillToMoveUp == null)
        {
            return new ResponseLogicModel<BaseDatabaseModel<SkillDataDatabaseModel>>
            {
                Success = false,
                Message = "Failed to move up the skill because it does not exist.",
                Body = null
            };
        }
        if (skillToMoveUp.Metadata.Order == 0)
        {
            return new ResponseLogicModel<BaseDatabaseModel<SkillDataDatabaseModel>>
            {
                Success = false,
                Message = "Failed to move up the skill because it is the first element.",
                Body = null
            };
        }
        var skills = skillToMoveUp.Metadata.ParentId.HasValue
            ? service.ReadByParentId(skillToMoveUp.Metadata.ParentId.Value)
            : service.ReadAll();
        var prevSkill = skills.FirstOrDefault(
            item => item.Metadata.Order == skillToMoveUp.Metadata.Order - 1
        );
        if (prevSkill == null)
        {
            return new ResponseLogicModel<BaseDatabaseModel<SkillDataDatabaseModel>>
            {
                Success = false,
                Message = "Failed to move up the skill because the previous skill does not exist.",
                Body = null
            };
        }
        var skillToMoveUpOrder = skillToMoveUp.Metadata.Order;
        skillToMoveUp.Metadata.Order = prevSkill.Metadata.Order;
        prevSkill.Metadata.Order = skillToMoveUpOrder;
        service.Update(skillToMoveUp);
        service.Update(prevSkill);
        return new ResponseLogicModel<BaseDatabaseModel<SkillDataDatabaseModel>>
        {
            Success = true,
            Message = "The skill was moved up correctly.",
            Body = skillToMoveUp
        };
    }
    public ResponseLogicModel<BaseDatabaseModel<SkillDataDatabaseModel>> MoveDown(int id)
    {
        var skillToMoveDown = service.ReadById(id);
        if (skillToMoveDown == null)
        {
            return new ResponseLogicModel<BaseDatabaseModel<SkillDataDatabaseModel>>
            {
                Success = false,
                Message = "Failed to move down the skill because it does not exist.",
                Body = null
            };
        }
        var skills = skillToMoveDown.Metadata.ParentId.HasValue
            ? service.ReadByParentId(skillToMoveDown.Metadata.ParentId.Value)
            : service.ReadAll();
        var lastOrder = skills.Aggregate(
            0,
            (acc, curr) => curr.Metadata.Order > acc
                ? curr.Metadata.Order
                : acc
        );
        if (skillToMoveDown.Metadata.Order == lastOrder)
        {
            return new ResponseLogicModel<BaseDatabaseModel<SkillDataDatabaseModel>>
            {
                Success = false,
                Message = "Failed to move down the skill because it is the last element.",
                Body = null
            };
        }
        var nextSkill = skills.FirstOrDefault(
            skill => skill.Metadata.Order == skillToMoveDown.Metadata.Order + 1
        );
        if (nextSkill == null)
        {
            return new ResponseLogicModel<BaseDatabaseModel<SkillDataDatabaseModel>>
            {
                Success = false,
                Message = "Failed to move down the skill because the next skill does not exist.",
                Body = null
            };
        }
        var skillToMoveDownOrder = skillToMoveDown.Metadata.Order;
        skillToMoveDown.Metadata.Order = nextSkill.Metadata.Order;
        nextSkill.Metadata.Order = skillToMoveDownOrder;
        service.Update(skillToMoveDown);
        service.Update(nextSkill);
        return new ResponseLogicModel<BaseDatabaseModel<SkillDataDatabaseModel>>
        {
            Success = true,
            Message = "The skill was moved down correctly.",
            Body = skillToMoveDown
        };
    }
}