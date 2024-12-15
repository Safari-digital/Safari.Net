namespace Digital.Net.Authentication.Attributes;

[Flags]
public enum AuthorizeType
{
    ApiKey,
    Jwt
}