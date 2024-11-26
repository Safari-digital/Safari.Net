﻿using Digital.Net.Core.Interval;
using Digital.Net.Entities.Repositories;
using Digital.Net.Entities.Test.TestUtilities;
using Digital.Net.Entities.Test.TestUtilities.Models;
using Digital.Net.TestTools;
using Digital.Net.TestTools.Data;
using Digital.Net.TestTools.Data.Factories;
using Microsoft.AspNetCore.JsonPatch;

namespace Digital.Net.Entities.Test.Services;

public class EntityServiceTest : UnitTest
{
    private readonly DataFactory<FakeUser> _userFactory;
    private readonly Repository<FakeUser, TestContext> _userRepository;
    private readonly FakeUserService _userService;

    public EntityServiceTest()
    {
        var context = new SqliteMemoryDb<TestContext>().Context;
        _userRepository = new Repository<FakeUser, TestContext>(context);
        _userFactory = new DataFactory<FakeUser>(_userRepository);
        _userService = new FakeUserService(_userRepository);
    }

    [Fact]
    public void GetSchema_ReturnsCorrectSchema_WhenEntityHasProperties()
    {
        var schema = _userService.GetSchema();
        Assert.Equal("Username", schema[0].Name);
    }

    public void Get_ReturnsMappedModelWithCorrectPagination_WhenQueryIsValid()
    {
        const int total = 10;
        const int index = 1;
        const int size = 5;
        _userFactory.CreateMany(total);
        var query = new FakeUserQuery { Index = index, Size = size };
        var result = _userService.Get<FakeUserModel>(query);
        Assert.NotNull(result);
        Assert.Equal(total, result.Total);
        Assert.Equal(size, result.Size);
        Assert.Equal(index, result.Index);
        Assert.Equal(size, result.Count);
    }

    [Fact]
    public void Get_ReturnsCorrectItems_WhenFilteredWithMutationDates()
    {
        for (var i = 1; i < 3; i++)
            _userFactory.Create(new FakeUser { CreatedAt = DateTime.UtcNow.AddDays(-i + 1) });

        var query = new FakeUserQuery { CreatedAt = DateTime.Now.AddDays(-1) };
        var result = _userService.Get<FakeUserModel>(query);
        Assert.Equal(2, result.Count);
    }

    [Fact]
    public void Get_ReturnsCorrectItems_WhenFilteredWithMutationDateRanges()
    {
        var now = DateTime.UtcNow;
        var users = _userFactory.CreateMany(5);
        var query = new FakeUserQuery
        {
            CreatedIn = new DateRange { From = now, To = now.AddDays(2) }
        };
        foreach (var user in users)
        {
            var i = users.IndexOf(user);
            user.CreatedAt = now.AddDays(i);
            _userRepository.Save();
        }

        var result = _userService.Get<FakeUserModel>(query);
        Assert.Equal(3, result.Count);
    }

    [Fact]
    public void Get_ReturnsCorrectItems_WhenIndexInSecondPage()
    {
        const int total = 10;
        const int index = 2;
        const int size = 5;
        var users = _userFactory.CreateMany(total);
        var query = new FakeUserQuery
        {
            Index = index,
            Size = size,
            OrderBy = "CreatedAt"
        };
        foreach (var user in users)
        {
            var i = users.IndexOf(user) + 1;
            user.Username = $"User{i}";
            user.CreatedAt = DateTime.Now.AddDays(-total + i);
            _userRepository.Save();
        }

        var result = _userService.Get<FakeUserModel>(query);
        Assert.Equal("User6", result.Value.First().Username);
    }

    [Fact]
    public void Get_ReturnsError_WhenInvalidOrder()
    {
        const int total = 10;
        var query = new FakeUserQuery { OrderBy = "Lol" };
        _userFactory.CreateMany(total);
        var result = _userService.Get<FakeUserModel>(query);
        Assert.True(result.HasError);
        Assert.NotEmpty(result.Errors);
    }

    [Fact]
    public async Task Patch_ReturnsMappedModel_WhenQueryIsValid()
    {
        var user = await _userFactory.CreateAsync();
        var patch = new JsonPatchDocument<FakeUser>();
        patch.Replace(u => u.Username, "NewUsername");
        var result = await _userService.Patch(patch, user.Id);
        var updatedUser = await _userRepository.GetByIdAsync(user.Id);
        Assert.NotNull(result);
        Assert.Equal("NewUsername", updatedUser?.Username);
    }

    [Fact]
    public async Task Patch_ReturnsError_WhenEntityNotFound()
    {
        var patch = new JsonPatchDocument<FakeUser>();
        patch.Replace(u => u.Username, "NewUsername");
        var result = await _userService.Patch(patch, Guid.NewGuid());
        Assert.True(result.HasError);
    }

    [Fact]
    public async Task Patch_ReturnsError_WhenInvalidRegex()
    {
        var user = await _userFactory.CreateAsync();
        var patch = new JsonPatchDocument<FakeUser>();
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
        var patch = new JsonPatchDocument<FakeUser>();
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
        var patch = new JsonPatchDocument<FakeUser>();
        patch.Replace(u => u.Role, new FakeRole());
        var result = await _userService.Patch(patch, user.Id);
        Assert.True(result.HasError);
    }

    [Fact]
    public async Task Create_ReturnsSuccess_WhenEntityIsValid()
    {
        var user = new FakeUser
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
        var user = new FakeUser();
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