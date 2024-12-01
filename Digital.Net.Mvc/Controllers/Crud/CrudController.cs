using System.Text.Json;
using Digital.Net.Core.Messages;
using Digital.Net.Core.Models;
using Digital.Net.Entities.Models;
using Digital.Net.Entities.Services;
using Digital.Net.Mvc.Formatters;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Digital.Net.Mvc.Controllers.Crud;

[Route("[controller]")]
public abstract class CrudController<T, TDto, TPayload>(
    IHttpContextAccessor contextAccessor,
    IEntityService<T> entityService
) : ControllerBase
    where T : EntityBase
    where TDto : class
    where TPayload : class
{
    private readonly HttpContext Context =
        contextAccessor.HttpContext ?? throw new NullReferenceException("Http Context is not defined");

    [HttpGet("/schema")]
    public ActionResult<Result<List<SchemaProperty<T>>>> GetSchema() =>
        OnAuthorize(Context) ? Ok(entityService.GetSchema()) : Unauthorized();

    [HttpGet("{id}")]
    public ActionResult<Result<TDto>> GetById(string id)
    {
        if (!OnAuthorize(Context))
            return Unauthorized();
        
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
        if (!OnAuthorize(Context))
            return Unauthorized();
        
        var result = await entityService.Create(Mapper.Map<TPayload, T>(payload));
        return result.HasError ? BadRequest(result) : Ok(result);
    }

    [HttpPatch("{id}")]
    public async Task<ActionResult<Result>> Patch(string id, [FromBody] JsonElement patch)
    {
        if (!OnAuthorize(Context))
            return Unauthorized();
        
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
        if (!OnAuthorize(Context))
            return Unauthorized();
            
        var result = new Result();

        if (Guid.TryParse(id, out var guidId))
            result = await entityService.Delete(guidId);
        else if (int.TryParse(id, out var intId))
            result = await entityService.Delete(intId);
        else
            result.AddError(new KeyNotFoundException("Entity not found."));

        return result.HasError ? NotFound(result) : Ok(result);
    }

    public bool IsGetSchemaExecution() => ControllerContext.ActionDescriptor.ActionName == "GetSchema";
    public bool IsGetExecution() => ControllerContext.ActionDescriptor.ActionName == "GetById";
    public bool IsPostExecution() => ControllerContext.ActionDescriptor.ActionName == "Post";
    public bool IsPatchExecution() => ControllerContext.ActionDescriptor.ActionName == "Patch";
    public bool IsDeleteExecution() => ControllerContext.ActionDescriptor.ActionName == "Delete";

    public virtual bool OnAuthorize(HttpContext context) => true;
}