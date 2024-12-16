using Authorizations.Application.Abstractions;
using DistributeSystem.Contract.Abstractions.Message;
using DistributeSystem.Contract.Abstractions.Shared;
using DistributeSystem.Contract.Services.V1.Identity;
using System.Security.Claims;
using static Authorizations.Domain.Exceptions.IdentityException;

namespace Authorizations.Application.Queries.Identity;

public class GetRefreshTokenQueryHandler : IQueryHandler<Query.Token, Response.Authenicated>
{
    private readonly IJwtTokenService _jwtTokenService;
    private readonly ICacheService _cacheService;

    public GetRefreshTokenQueryHandler(IJwtTokenService jwtTokenService, ICacheService cacheService)
    {
        _jwtTokenService = jwtTokenService;
        _cacheService = cacheService;
    }

    public async Task<Result<Response.Authenicated>> Handle(Query.Token request, CancellationToken cancellationToken)
    {
        var accessToken = request.AccessToken;
        var refreshToken = request.RefreshToken;

        var principal = _jwtTokenService.GetPrincipalFromExpiredToken(accessToken);
        var emailKey = principal.FindFirst(ClaimTypes.Email).ToString();

        var authenticated = await _cacheService.GetAsync<Response.Authenicated>(emailKey);
        if (authenticated is null || authenticated.RefreshToken != refreshToken || authenticated.RefreshTokenExpiryTime <= DateTime.Now)
            throw new TokenException("Request token invalid!");

        var newAccessToken = _jwtTokenService.GenerateAccessToken(principal.Claims);
        var newRefreshToken = _jwtTokenService.GenerateRefreshToken();

        var newAuthenticated = new Response.Authenicated()
        {
            AccessToken = newAccessToken,
            RefreshToken = newRefreshToken,
            RefreshTokenExpiryTime = DateTime.Now.AddMinutes(5)
        };

        await _cacheService.SetAsync(emailKey, newAuthenticated, cancellationToken);

        return Result.Success(newAuthenticated);
    }
}
