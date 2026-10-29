using Microsoft.AspNetCore.Mvc;
using ResumeMakerApi.Models.Database.Data;
using ResumeMakerApi.Services.Logic;

namespace ResumeMakerApi.Controllers;

[ApiController]
[Route("api/[controller]")]
public class SkillController(SkillLogicService service) : ControllerBase
{
    [HttpPost("{id}/select")]
    public IActionResult Select(int id)
    {
        var result = service.Select(id);
        return result.Success ? Ok(result) : NotFound(result);
    }

    [HttpPost("{id}/deselect")]
    public IActionResult Deselect(int id)
    {
        var result = service.Deselect(id);
        return result.Success ? Ok(result) : NotFound(result);
    }

    [HttpPost]
    public IActionResult Create([FromBody] SkillDataDatabaseModel data, [FromQuery] int? parentId)
    {
        var result = service.Create(data, parentId);
        return result.Success ? Ok(result) : BadRequest(result);
    }

    [HttpGet("{id}")]
    public IActionResult ReadById(int id)
    {
        var result = service.ReadById(id);
        return result.Success ? Ok(result) : NotFound(result);
    }

    [HttpGet]
    public IActionResult ReadByParentId([FromQuery] int? parentId)
    {
        var result = service.ReadByParentId(parentId);
        return result.Success ? Ok(result) : NotFound(result);
    }

    [HttpPut("{id}")]
    public IActionResult Update(int id, [FromBody] SkillDataDatabaseModel data)
    {
        var result = service.Update(id, data);
        return result.Success ? Ok(result) : NotFound(result);
    }

    [HttpDelete("{id}")]
    public IActionResult Delete(int id)
    {
        var result = service.Delete(id);
        return result.Success ? Ok(result) : NotFound(result);
    }

    [HttpPost("{id}/move-up")]
    public IActionResult MoveUp(int id)
    {
        var result = service.MoveUp(id);
        return result.Success ? Ok(result) : BadRequest(result);
    }

    [HttpPost("{id}/move-down")]
    public IActionResult MoveDown(int id)
    {
        var result = service.MoveDown(id);
        return result.Success ? Ok(result) : BadRequest(result);
    }
}