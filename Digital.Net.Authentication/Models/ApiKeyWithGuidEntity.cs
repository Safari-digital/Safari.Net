namespace Digital.Net.Authentication.Models;

public class ApiKeyWithGuidEntity : ApiKeyEntity
{
    protected ApiKeyWithGuidEntity(string? key = null, DateTime? expiredAt = null) : base(key, expiredAt)
    {
        Id = Guid.NewGuid();
    }

    public new Guid Id { get; init; }
}