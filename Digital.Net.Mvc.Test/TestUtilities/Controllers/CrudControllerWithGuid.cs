using Digital.Net.Entities.Services;
using Digital.Net.Mvc.Controllers.Crud;
using InternalTestProgram.Models;

namespace Digital.Net.Mvc.Test.TestUtilities.Controllers;

public class CrudControllerWithGuid(IEntityService<TestGuidEntity> entityService)
    : CrudController<TestGuidEntity, TestGuidEntityDto, TestGuidEntityPayload>(entityService);