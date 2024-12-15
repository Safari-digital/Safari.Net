using Digital.Net.Entities.Services;
using Digital.Net.Mvc.Controllers.Pagination;
using Digital.Net.Mvc.Test.TestUtilities.Controllers;
using Digital.Net.TestTools;
using InternalTestProgram.Models;
using Microsoft.AspNetCore.Mvc;
using Moq;
using Xunit;

namespace Digital.Net.Mvc.Test.Controllers.Crud;

public class CrudControllerTest : UnitTest
{
    private readonly Mock<IEntityService<TestIdEntity>> _idEntityServiceMock = new();
    private readonly Mock<IEntityService<TestGuidEntity>> _guidEntityServiceMock = new();

    private readonly CrudControllerWithId _crudIdController;
    private readonly CrudControllerWithGuid _crudGuidController;

    public CrudControllerTest()
    {
        _idEntityServiceMock
            .Setup(x => x.Get<TestIdEntityDto>(It.IsAny<int>()))
            .Returns(new QueryResult<TestIdEntityDto>());
        _guidEntityServiceMock
            .Setup(x => x.Get<TestGuidEntityDto>(It.IsAny<Guid>()))
            .Returns(new QueryResult<TestGuidEntityDto>());

        _crudIdController = new CrudControllerWithId(_idEntityServiceMock.Object);
        _crudGuidController = new CrudControllerWithGuid(_guidEntityServiceMock.Object);
    }

    [Fact]
    public void GetById_AsInt_ShouldCallGetByWitchCorrectSignature()
    {
        const int id = 1;
        _crudIdController.GetById(1.ToString());
        _idEntityServiceMock.Verify(x => x.Get<TestIdEntityDto>(id), Times.Once);
    }

    [Fact]
    public void GetById_AsGuid_ShouldCallGetByWitchCorrectSignature()
    {
        var id = Guid.NewGuid();
        _crudGuidController.GetById(id.ToString());
        _guidEntityServiceMock.Verify(x => x.Get<TestGuidEntityDto>(id), Times.Once);
    }

    [Fact]
    public void GetById_InvalidId_ShouldReturnNotFound()
    {
        var result = _crudGuidController.GetById("invalidId");
        Assert.IsType<NotFoundObjectResult>(result.Result);
    }
}