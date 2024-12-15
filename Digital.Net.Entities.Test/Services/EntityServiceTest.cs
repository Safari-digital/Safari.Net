using Digital.Net.Entities.Repositories;
using Digital.Net.Entities.Services;
using Digital.Net.TestTools;
using Digital.Net.TestTools.Data;
using Digital.Net.TestTools.Data.Factories;
using InternalTestProgram;
using InternalTestProgram.Models;
using Microsoft.AspNetCore.JsonPatch;

namespace Digital.Net.Entities.Test.Services;

public class EntityServiceTest : UnitTest
{
    private readonly DataFactory<TestUser> _userFactory;
    private readonly Repository<TestUser> _userRepository;
    private readonly IEntityService<TestUser> _userService;

    public EntityServiceTest()
    {
        var context = new SqliteMemoryDb<TestContext>().Context;
        _userRepository = new Repository<TestUser>(context);
        _userFactory = new DataFactory<TestUser>(_userRepository);
        _userService = new EntityService<TestUser>(_userRepository);
    }

    [Fact]
    public void GetSchema_ReturnsCorrectSchema_WhenEntityHasProperties()
    {
        var schema = _userService.GetSchema();
        Assert.Equal("Username", schema[0].Name);
    }

    [Fact]
    public async Task Patch_ReturnsMappedModel_WhenQueryIsValid()
    {
        var user = await _userFactory.CreateAsync();
        var patch = new JsonPatchDocument<TestUser>();
        patch.Replace(u => u.Username, "NewUsername");
        var result = await _userService.Patch(patch, user.Id);
        var updatedUser = await _userRepository.GetByIdAsync(user.Id);
        Assert.NotNull(result);
        Assert.Equal("NewUsername", updatedUser?.Username);
    }

    [Fact]
    public async Task Patch_ReturnsError_WhenEntityNotFound()
    {
        var patch = new JsonPatchDocument<TestUser>();
        patch.Replace(u => u.Username, "NewUsername");
        var result = await _userService.Patch(patch, Guid.NewGuid());
        Assert.True(result.HasError);
    }

    [Fact]
    public async Task Patch_ReturnsError_WhenInvalidRegex()
    {
        var user = await _userFactory.CreateAsync();
        var patch = new JsonPatchDocument<TestUser>();
        patch.Replace(u => u.Username, "to");
        var result = await _userService.Patch(patch, user.Id);
        var updatedUser = await _userRepository.GetByIdAsync(user.Id);
        Assert.True(result.HasError);
        Assert.NotEqual("", updatedUser?.Username);
    }
    
    [Fact]
    public async Task Patch_ReturnsError_WhenUniqueConstraint()
    {
        var user = await _userFactory.CreateAsync();
        var user2 = await _userFactory.CreateAsync();
        var patch = new JsonPatchDocument<TestUser>();
        patch.Replace(u => u.Username, user2.Username);
        var result = await _userService.Patch(patch, user.Id);
        var updatedUser = await _userRepository.GetByIdAsync(user.Id);
        Assert.True(result.HasError);
        Assert.NotEqual(user2.Username, updatedUser?.Username);
    }
    
    [Fact]
    public async Task Patch_ReturnsError_WhenPatchingReadOnlyField()
    {
        var user = await _userFactory.CreateAsync();
        var patch = new JsonPatchDocument<TestUser>();
        patch.Replace(u => u.Role, new TestRole());
        var result = await _userService.Patch(patch, user.Id);
        Assert.True(result.HasError);
    }

    [Fact]
    public async Task Create_ReturnsSuccess_WhenEntityIsValid()
    {
        var user = new TestUser
        {
            Username = "NewUser",
            Password = "SecretPassword123!",
            Email = "user@mail.com"
        };
        var result = await _userService.Create(user);
        var createdUser = await _userRepository.GetByIdAsync(user.Id);
        Assert.False(result.HasError);
        Assert.NotNull(createdUser);
        Assert.Equal("NewUser", createdUser?.Username);
    }

    [Fact]
    public async Task Create_ReturnsError_WhenEntityIsInvalid()
    {
        var user = new TestUser();
        var result = await _userService.Create(user);
        Assert.True(result.HasError);
    }

    [Fact]
    public async Task Delete_ReturnsSuccess_WhenEntityExists()
    {
        var user = await _userFactory.CreateAsync();
        var result = await _userService.Delete(user.Id);
        var deletedUser = await _userRepository.GetByIdAsync(user.Id);
        Assert.False(result.HasError);
        Assert.Null(deletedUser);
    }

    [Fact]
    public async Task Delete_ReturnsError_WhenEntityDoesNotExist()
    {
        var result = await _userService.Delete(Guid.NewGuid());
        Assert.True(result.HasError);
    }
}