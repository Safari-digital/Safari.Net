using System.Linq.Expressions;
using Digital.Net.Entities.Models;

namespace Digital.Net.Entities.Repositories;

/// <summary>
///     Interface for a repository.
/// </summary>
/// <typeparam name="T">The entity type. (Must inherit from EntityBase)</typeparam>
public interface IRepository<T>
    where T : Entity
{
    /// <summary>
    ///     Create a new entity
    /// </summary>
    /// <param name="entity">The entity to create</param>
    public void Create(T entity);

    /// <summary>
    ///     Create a new entity asynchronously.
    /// </summary>
    /// <param name="entity">The entity to create</param>
    public Task CreateAsync(T entity);

    /// <summary>
    ///     Delete an entity
    /// </summary>
    /// <param name="entity">The entity to delete</param>
    public void Delete(T entity);

    /// <summary>
    ///     Update an entity.
    /// </summary>
    /// <param name="entity">The entity to update</param>
    public void Update(T entity);

    /// <summary>
    ///     Update a range of entities.
    /// </summary>
    /// <param name="entities">The entities to update</param>
    public void UpdateRange(IEnumerable<T> entities);

    /// <summary>
    ///     Get entities based on a predicate.
    /// </summary>
    /// <param name="expression">The predicate to filter entities</param>
    /// <returns>Queryable of entities</returns>
    public IQueryable<T> Get(Expression<Func<T, bool>> expression);

    /// <summary>
    ///     Save changes to the database.
    /// </summary>
    public void Save();

    /// <summary>
    ///     Save changes to the database asynchronously.
    /// </summary>
    public Task SaveAsync();

    /// <summary>
    ///     Get an entity by its id.
    /// </summary>
    /// <param name="id">The primary key of the entity.</param>
    /// <returns>The entity</returns>
    T? GetById(int? id);

    /// <summary>
    ///     Get an entity by its id asynchronously.
    /// </summary>
    /// <param name="id">The primary key of the entity.</param>
    /// <returns>The entity</returns>
    T? GetById(Guid? id);

    /// <summary>
    ///     Get an entity by its id asynchronously.
    /// </summary>
    /// <param name="id">The primary key of the entity.</param>
    /// <returns>The entity</returns>
    Task<T?> GetByIdAsync(int? id);

    /// <summary>
    ///     Get an entity by its id asynchronously.
    /// </summary>
    /// <param name="id">The primary key of the entity.</param>
    /// <returns>The entity</returns>
    Task<T?> GetByIdAsync(Guid? id);

    /// <summary>
    ///     Get the count of entities based on a predicate.
    /// </summary>
    /// <param name="expression">The predicate to filter entities</param>
    /// <returns>The count of entities</returns>
    int Count(Expression<Func<T, bool>> expression);

    /// <summary>
    ///     Get the count of entities based on a predicate asynchronously.
    /// </summary>
    /// <param name="expression">The predicate to filter entities</param>
    /// <returns>The count of entities</returns>
    Task<int> CountAsync(Expression<Func<T, bool>> expression);
}
