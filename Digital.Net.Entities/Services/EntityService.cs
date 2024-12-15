using System.Linq.Expressions;
using Digital.Net.Core.Messages;
using Digital.Net.Core.Models;
using Digital.Net.Entities.Models;
using Digital.Net.Entities.Repositories;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.EntityFrameworkCore;

namespace Digital.Net.Entities.Services;

public class EntityService<T>(IRepository<T> repository) : IEntityService<T> where T : Entity
{
    public List<SchemaProperty<T>> GetSchema() =>
        typeof(T)
            .GetProperties()
            .Select(property => new SchemaProperty<T>(property))
            .ToList();

    public Result<TModel> Get<TModel>(Guid? id) where TModel : class => Get<TModel>(repository.GetById(id));
    public Result<TModel> Get<TModel>(int id) where TModel : class => Get<TModel>(repository.GetById(id));

    private static Result<TModel> Get<TModel>(T? entity) where TModel : class
    {
        var result = new Result<TModel>();
        if (entity is null)
            return result.AddError(new KeyNotFoundException("Entity not found."));
        result.Value = Mapper.MapFromConstructor<T, TModel>(entity);
        return result;
    }

    public async Task<Result> Patch(JsonPatchDocument<T> patch, Guid? id) =>
        await Patch(patch, await repository.GetByIdAsync(id));

    public async Task<Result> Patch(JsonPatchDocument<T> patch, int id) =>
        await Patch(patch, await repository.GetByIdAsync(id));

    private async Task<Result> Patch(JsonPatchDocument<T> patch, T? entity)
    {
        var result = new Result();
        if (entity is null)
            return result.AddError(new KeyNotFoundException("Entity not found."));
        try
        {
            foreach (var o in patch.Operations)
            {
                var key = o.path[1..];
                ValidatePayload(o.value, o.path, x => x.Name == key);
            }

            patch.ApplyTo(entity);
            repository.Update(entity);
            await OnPatch(entity);
            await repository.SaveAsync();
        }
        catch (Exception e)
        {
            result.AddError(e);
        }

        return result;
    }

    public async Task<Result> Delete(Guid? id) => await Delete(await repository.GetByIdAsync(id));
    public async Task<Result> Delete(int id) => await Delete(await repository.GetByIdAsync(id));

    private async Task<Result> Delete(T? entity)
    {
        var result = new Result();
        if (entity is null)
            return result.AddError(new KeyNotFoundException("Entity not found."));
        try
        {
            await OnDelete(entity);
            repository.Delete(entity);
            await repository.SaveAsync();
        }
        catch (Exception e)
        {
            result.AddError(e);
        }

        return result;
    }

    public async Task<Result> Create(T entity)
    {
        var result = new Result();
        try
        {
            foreach (var property in entity.GetType().GetProperties())
                ValidatePayload(property.GetValue(entity), property.Name, x => x.Name == property.Name);

            await OnCreate(entity);
            await repository.CreateAsync(entity);
            await repository.SaveAsync();
        }
        catch (Exception e)
        {
            result.AddError(e);
        }

        return result;
    }

    private void ValidatePayload(object? value, string path, Expression<Func<SchemaProperty<T>, bool>> schemaPredicate)
    {
        var prop = GetSchema().First(schemaPredicate.Compile());

        if (value is null)
            return;

        if ((prop.IsIdentity || prop.IsForeignKey) && value.ToString() is "00000000-0000-0000-0000-000000000000" or "0")
            return;

        if (path is "CreatedAt" or "UpdatedAt" && (DateTime)value == DateTime.MinValue)
            return;

        if (prop.IsIdentity || prop.IsReadOnly)
            throw new InvalidOperationException($"{path}: This field is read-only.");

        if (prop.IsRequired && value is null)
            throw new InvalidOperationException($"{path}: This field is required and cannot be null.");

        if (prop is { IsRequired: true, Type: "String" } && string.IsNullOrWhiteSpace(value.ToString()))
            throw new InvalidOperationException($"{path}: This field is required and cannot be empty.");

        if (prop is { MaxLength: > 0, Type: "String" } && value.ToString()?.Length > prop.MaxLength)
            throw new InvalidOperationException($"{path}: Maximum length exceeded.");

        if (prop.IsUnique &&
            repository.Get(x => EF.Property<object>(x, prop.Name).Equals(value)).Any())
            throw new InvalidOperationException($"{path}: This value violates a unique constraint.");

        if (prop.RegexValidation is not null &&
            !prop.RegexValidation.IsMatch(value.ToString() ?? ""))
            throw new InvalidOperationException($"{path}: This value does not meet the requirements.");
    }

    protected virtual Task OnCreate(T entity) => Task.CompletedTask;
    protected virtual Task OnPatch(T entity) => Task.CompletedTask;
    protected virtual Task OnDelete(T entity) => Task.CompletedTask;
}
