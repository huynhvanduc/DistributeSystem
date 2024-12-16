using Authorizations.Application.Abstractions;
using DistributeSystem.Contract.Abstractions.Message;
using DistributeSystem.Contract.Abstractions.Shared;
using DistributeSystem.Contract.Services.V1.Identity;
using System.Security.Claims;

namespace Authorizations.Application.Commands.Identity;

public class RevokeTokenCommandHandler : ICommandHandler<Command.Revoke>
{
    private readonly ICacheService _cacheService;
    private readonly IJwtTokenService _jwtTokenService;

    public RevokeTokenCommandHandler(ICacheService cacheService, IJwtTokenService jwtTokenService)
    {
        _cacheService = cacheService;
        _jwtTokenService = jwtTokenService;
    }

    public async Task<Result> Handle(Command.Revoke request, CancellationToken cancellationToken)
    {
        var AccessToken = request.AccessToken;
        var principal = _jwtTokenService.GetPrincipalFromExpiredToken(AccessToken);
        var emailKey = principal.FindFirstValue(ClaimTypes.Email).ToString();

        var authenticated = await _cacheService.GetAsync<Response.Authenicated>(emailKey);
        if (authenticated is null)
            throw new Exception("Can not get value from Redis");

        await _cacheService.RemoveAsync(emailKey, cancellationToken);

        return Result.Success();
    }
}
