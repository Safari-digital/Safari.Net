using Digital.Net.Entities.Services;
using Digital.Net.Mvc.Controllers.Crud;
using InternalTestProgram.Models;

namespace Digital.Net.Mvc.Test.TestUtilities.Controllers;

public class CrudControllerWithId(IEntityService<TestIdEntity> entityService)
    : CrudController<TestIdEntity, TestIdEntityDto, TestIdEntityPayload>(entityService);