namespace Digital.Net.Authentication.Models;

public class ApiKeyWithGuidEntity : ApiKeyEntity
{
    public new Guid Id { get; init; }
}