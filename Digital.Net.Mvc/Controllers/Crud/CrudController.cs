using System.Text.Json;
using Digital.Net.Core.Messages;
using Digital.Net.Core.Models;
using Digital.Net.Entities.Models;
using Digital.Net.Entities.Services;
using Digital.Net.Mvc.Formatters;
using Microsoft.AspNetCore.Mvc;

namespace Digital.Net.Mvc.Controllers.Crud;

[ApiController, Route("[controller]")]
public abstract class CrudController<T, TDto, TPayload>(
    IEntityService<T> entityService
) : ControllerBase
    where T : Entity
    where TDto : class
    where TPayload : class
{
    [HttpGet("schema")]
    public ActionResult<Result<List<SchemaProperty<T>>>> GetSchema() => Ok(entityService.GetSchema());

    [HttpGet("{id}")]
    public ActionResult<Result<TDto>> GetById(string id)
    {
        var result = new Result<TDto>();

        if (Guid.TryParse(id, out var guidId))
            result = entityService.Get<TDto>(guidId);
        else if (int.TryParse(id, out var intId))
            result = entityService.Get<TDto>(intId);
        else
            result.AddError(new KeyNotFoundException("Entity not found."));

        return result.HasError ? NotFound(result) : Ok(result);
    }

    [HttpPost("")]
    public async Task<ActionResult<Result>> Post([FromBody] TPayload payload)
    {
        var result = await entityService.Create(Mapper.Map<TPayload, T>(payload));
        return result.HasError ? BadRequest(result) : Ok(result);
    }

    [HttpPatch("{id}")]
    public async Task<ActionResult<Result>> Patch(string id, [FromBody] JsonElement patch)
    {
        var result = new Result();

        if (Guid.TryParse(id, out var guidId))
            result = await entityService.Patch(JsonFormatter.GetPatchDocument<T>(patch), guidId);
        else if (int.TryParse(id, out var intId))
            result = await entityService.Patch(JsonFormatter.GetPatchDocument<T>(patch), intId);
        else
            result.AddError(new KeyNotFoundException("Entity not found."));

        if (result.HasError && result.Errors[0].GetType() == typeof(KeyNotFoundException))
            return NotFound(result);
        if (result.HasError && result.Errors[0].GetType() == typeof(InvalidOperationException))
            return BadRequest(result);

        return Ok(result);
    }

    [HttpDelete("{id}")]
    public async Task<ActionResult<Result>> Delete(string id)
    {
        var result = new Result();

        if (Guid.TryParse(id, out var guidId))
            result = await entityService.Delete(guidId);
        else if (int.TryParse(id, out var intId))
            result = await entityService.Delete(intId);
        else
            result.AddError(new KeyNotFoundException("Entity not found."));

        return result.HasError ? NotFound(result) : Ok(result);
    }

    [NonAction]
    public bool IsGetSchemaExecution() => ControllerContext.ActionDescriptor.ActionName == "GetSchema";
    [NonAction]
    public bool IsGetExecution() => ControllerContext.ActionDescriptor.ActionName == "GetById";
    [NonAction]
    public bool IsPostExecution() => ControllerContext.ActionDescriptor.ActionName == "Post";
    [NonAction]
    public bool IsPatchExecution() => ControllerContext.ActionDescriptor.ActionName == "Patch";
    [NonAction]
    public bool IsDeleteExecution() => ControllerContext.ActionDescriptor.ActionName == "Delete";
}