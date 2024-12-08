using Digital.Net.Entities.Services;
using Digital.Net.Mvc.Controllers.Crud;
using InternalTestUtilities.Models;
using Microsoft.AspNetCore.Http;

namespace Digital.Net.Mvc.Test.TestUtilities.Controllers;

public class CrudControllerWithGuid(
    IHttpContextAccessor contextAccessor,
    IEntityService<TestGuidEntity> entityService
) : CrudController<TestGuidEntity, TestGuidEntityDto, TestGuidEntityPayload>(contextAccessor, entityService);