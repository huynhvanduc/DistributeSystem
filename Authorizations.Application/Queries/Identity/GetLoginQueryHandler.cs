using Authorizations.Application.Abstractions;
using DistributeSystem.Contract.Abstractions.Message;
using DistributeSystem.Contract.Abstractions.Shared;
using DistributeSystem.Contract.Services.V1.Identity;
using System.Security.Claims;

namespace Authorizations.Application.Queries.Identity;

public class GetLoginQueryHandler : IQueryHandler<Query.Login, Response.Authenicated>
{
    private readonly IJwtTokenService _jwtTokenService;
    private readonly ICacheService _cacheService;

    public GetLoginQueryHandler(IJwtTokenService jwtTokenService, ICacheService cacheService)
    {
        _jwtTokenService = jwtTokenService;
        _cacheService = cacheService;
    }

    public async Task<Result<Response.Authenicated>> Handle(Query.Login request, CancellationToken cancellationToken)
    {
        var claims = new List<Claim>
        {
            new Claim(ClaimTypes.Email, request.Email),
            new Claim(ClaimTypes.Role, "Senior .NET Leader")
        };

        var accessToken = _jwtTokenService.GenerateAccessToken(claims);
        var refreshToken = _jwtTokenService.GenerateRefreshToken();

        var response = new Response.Authenicated()
        {
            AccessToken = accessToken,
            RefreshToken = refreshToken,
            RefreshTokenExpiryTime = DateTime.Now.AddMinutes(5)
        };

        await _cacheService.SetAsync(request.Email, response);
        return Result.Success(response);
    }
}
