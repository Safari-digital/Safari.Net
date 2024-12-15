using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text.Json;
using Digital.Net.Authentication.Models.Authorizations;
using Digital.Net.Authentication.Services.Authorization;
using Digital.Net.Authentication.Services.Options;
using Digital.Net.Entities.Repositories;
using Digital.Net.Mvc.Services;
using Microsoft.IdentityModel.Tokens;

namespace Digital.Net.Authentication.Services.Authentication;

public class AuthenticationJwtService<TAuthorization>(
    IJwtOptionService jwtOptionService,
    IRepository<TAuthorization> tokenRepository,
    IHttpContextService httpContextService
) : IAuthenticationJwtService
    where TAuthorization : AuthorizationToken, new()
{
    public async Task RevokeTokenAsync(string token)
    {
        var record = tokenRepository.Get(t => t.Key == token).FirstOrDefault();
        if (record is null)
            return;

        record.ExpiredAt = DateTime.UtcNow;
        await tokenRepository.SaveAsync();
    }

    public async Task RevokeAllTokensAsync(Guid userId)
    {
        var records = tokenRepository.Get(t => t.ApiUserId == userId);
        foreach (var record in records)
            record.ExpiredAt = DateTime.UtcNow;
        await tokenRepository.SaveAsync();
    }

    public string GenerateBearerToken(Guid userId)
    {
        var content = new TokenContent(userId, httpContextService.UserAgent);
        return SignToken(content, jwtOptionService.GetBearerTokenExpirationDate());
    }

    public string GenerateRefreshToken(Guid userId)
    {
        var content = new TokenContent(userId, httpContextService.UserAgent);
        var tokenExpiration = jwtOptionService.GetRefreshTokenExpirationDate();
        var token = SignToken(content, tokenExpiration);

        HandleMaxConcurrentSessions(userId);
        tokenRepository.Create(new TAuthorization
        {
            Key = token,
            ApiUserId = userId,
            UserAgent = httpContextService.UserAgent,
            ExpiredAt = tokenExpiration
        });
        tokenRepository.Save();

        return token;
    }

    private string SignToken(TokenContent obj, DateTime expires)
    {
        var claims = new List<Claim> { new(JwtOptionService.ContentClaimType, JsonSerializer.Serialize(obj)) };
        var parameters = jwtOptionService.GetTokenParameters();
        var tokenHandler = new JwtSecurityTokenHandler();
        var token = tokenHandler.CreateToken(
            new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(claims),
                Expires = expires,
                SigningCredentials = new SigningCredentials(parameters.IssuerSigningKey, SecurityAlgorithms.HmacSha256),
                Issuer = parameters.ValidIssuer,
                Audience = parameters.ValidAudience
            }
        );
        return tokenHandler.WriteToken(token);
    }

    private void HandleMaxConcurrentSessions(Guid userId)
    {
        var maxTokenAllowed = jwtOptionService.MaxConcurrentSessions;
        var userTokens = tokenRepository.Get(t => t.ApiUserId == userId && t.ExpiredAt > DateTime.UtcNow);
        if (userTokens.Count() < maxTokenAllowed)
            return;

        var tokens = userTokens.OrderByDescending(t => t.CreatedAt).Skip(maxTokenAllowed);
        foreach (var token in tokens)
            token.ExpiredAt = DateTime.UtcNow;
    }
}