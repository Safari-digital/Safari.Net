using Digital.Net.Entities.Repositories;
using Digital.Net.TestTools;
using Digital.Net.TestTools.Data;
using Digital.Net.TestTools.Data.Factories;
using InternalTestProgram;
using InternalTestProgram.Models;

namespace Digital.Net.Entities.Test.Repositories;

public class RepositoryTest : UnitTest
{
    private readonly DataFactory<TestRole> _roleFactory;
    private readonly Repository<TestRole> _roleRepository;
    private readonly DataFactory<TestUser> _userFactory;
    private readonly Repository<TestUser> _userRepository;

    public RepositoryTest()
    {
        var context = new SqliteMemoryDb<TestContext>().Context;
        _userRepository = new Repository<TestUser>(context);
        _roleRepository = new Repository<TestRole>(context);
        _userFactory = new DataFactory<TestUser>(_userRepository);
        _roleFactory = new DataFactory<TestRole>(_roleRepository);
    }

    [Fact]
    public void GetById_ReturnsUser_WhenIdIsGuidAndUserExists()
    {
        var user = _userFactory.Create();
        var userById = _userRepository.GetById(user.Id);
        Assert.NotNull(userById);
        Assert.Equal(user.Id, userById.Id);
    }

    [Fact]
    public async void GetByIdAsync_ReturnsUser_WhenIdIsGuidAndUserExists()
    {
        var user = await _userFactory.CreateAsync();
        var userById = await _userRepository.GetByIdAsync(user.Id);
        Assert.NotNull(userById);
        Assert.Equal(user.Id, userById.Id);
    }

    [Fact]
    public void GetById_ReturnsUser_WhenIdIsIntAndUserExists()
    {
        var role = _roleFactory.Create();
        var roleById = _roleRepository.GetById(role.Id);
        Assert.NotNull(roleById);
        Assert.Equal(role.Id, roleById.Id);
    }

    [Fact]
    public async void GetByIdAsync_ReturnsUser_WhenIdIsIntAndUserExists()
    {
        var role = await _roleFactory.CreateAsync();
        var roleById = await _roleRepository.GetByIdAsync(role.Id);
        Assert.NotNull(roleById);
        Assert.Equal(role.Id, roleById.Id);
    }

    [Fact]
    public void Create_ShouldCreateUser()
    {
        var user = new TestUser { Username = "Garfield" };
        _userRepository.Create(user);
        _userRepository.Save();
        var created = _userRepository.Get(u => u.Username == user.Username).FirstOrDefault();
        Assert.NotNull(created);
        Assert.Equal(user.Username, created.Username);
    }

    [Fact]
    public async void CreateAsync_ShouldCreateUser()
    {
        var user = await _userFactory.CreateAsync();
        var createdUser = _userRepository.Get(u => u.Username == user.Username).FirstOrDefault();
        Assert.NotNull(createdUser);
        Assert.Equal(user.Username, createdUser.Username);
        Assert.True(createdUser.CreatedAt <= DateTime.UtcNow);
    }


    [Fact]
    public void Update_ShouldUpdateEntity()
    {
        var user = _userFactory.Create();
        user.Username = "John Arbuckle";
        _userRepository.Update(user);
        _userRepository.Save();
        var updatedUser = _userRepository.Get(u => u.Username == user.Username).FirstOrDefault()!;
        Assert.Equal(user.Username, updatedUser.Username);
        Assert.True(updatedUser.UpdatedAt <= DateTime.UtcNow);
    }

    [Fact]
    public void UpdateRange_ShouldUpdateEntities()
    {
        var users = _userFactory.CreateMany(15).Skip(10).ToList();
        foreach (var u in users)
            u.Username = "UpdatedUser" + users.FindIndex(usr => usr == u);

        _userRepository.UpdateRange(users);
        _userRepository.Save();

        var updatedUsers = _userRepository
            .Get(u => u.Username.StartsWith("UpdatedUser") && u.UpdatedAt <= DateTime.UtcNow).ToList();

        Assert.Equal(5, updatedUsers.Count);
    }

    [Fact]
    public void Delete_ShouldDeleteUser()
    {
        var user = _userFactory.Create();
        _userRepository.Delete(user);
        _userRepository.Save();
        var deleted = _userRepository.Get(u => u.Username == user.Username).FirstOrDefault();
        Assert.True(deleted is null);
    }
}