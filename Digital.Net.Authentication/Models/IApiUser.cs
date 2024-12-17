using Digital.Net.Entities.Models;

namespace Digital.Net.Authentication.Models;

public interface IApiUser : IEntity
{
    public string Password { get; }
    public string Login { get; }
    public bool IsActive { get; }
}