using Digital.Net.Entities.Models;
using Digital.Net.Entities.Repositories;

namespace Digital.Net.TestTools.Data.Factories;

public class DataFactory<T>(IRepository<T> repository)
    where T : Entity
{
    private readonly List<T> _entities = [];

    /// <summary>
    ///     Creates an entity and returns it.
    /// </summary>
    /// <param name="entity">Entity to create.</param>
    /// <returns>Created entity.</returns>
    /// <exception cref="InvalidOperationException"></exception>
    public T Create(T? entity = null)
    {
        entity ??= Activator.CreateInstance<T>();
        repository.Create(entity);
        repository.Save();
        _entities.Add(entity);
        var id = DataFactoryUtils.ResolveId(entity);
        if (Guid.TryParse(id.ToString(), out var guid))
            entity = repository.GetById(guid);
        if (int.TryParse(id.ToString(), out var intId))
            entity = repository.GetById(intId);
        return entity ?? throw new InvalidOperationException("Entity could not be created.");
    }

    /// <summary>
    ///     Creates an entity and returns it.
    /// </summary>
    /// <param name="entity">Entity to create.</param>
    /// <returns>Created entity.</returns>
    /// <exception cref="InvalidOperationException"></exception>
    public async Task<T> CreateAsync(T? entity = null)
    {
        entity ??= Activator.CreateInstance<T>();
        await repository.CreateAsync(entity);
        await repository.SaveAsync();
        _entities.Add(entity);
        var id = DataFactoryUtils.ResolveId(entity);
        if (Guid.TryParse(id.ToString(), out var guid))
            entity = await repository.GetByIdAsync(guid);
        if (int.TryParse(id.ToString(), out var intId))
            entity = await repository.GetByIdAsync(intId);
        return entity ?? throw new InvalidOperationException("Entity could not be created.");
    }

    /// <summary>
    ///     Creates a list of entities and returns them.
    /// </summary>
    /// <param name="count">Number of entities to create.</param>
    /// <returns>List of created entities.</returns>
    public List<T> CreateMany(int count)
    {
        var entities = new List<T>();
        for (var i = 0; i < count; i++)
            entities.Add(Create());
        return entities;
    }

    /// <summary>
    ///     Creates a list of entities and returns them.
    /// </summary>
    /// <param name="count">Number of entities to create.</param>
    /// <returns>List of created entities.</returns>
    public async Task<List<T>> CreateManyAsync(int count)
    {
        var entities = new List<T>();
        for (var i = 0; i < count; i++)
            entities.Add(await CreateAsync());
        return entities;
    }

    /// <summary>
    ///    Cleanup the repository.
    /// </summary>
    public void Dispose()
    {
        foreach (var entity in _entities)
            repository.Delete(entity);

        repository.Save();
    }
}
