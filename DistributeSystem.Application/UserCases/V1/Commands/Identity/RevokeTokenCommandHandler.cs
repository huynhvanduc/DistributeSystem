using DistributedSystem.Application.Abstractions;
using DistributeSystem.Application.Abstractions;
using DistributeSystem.Contract.Abstractions.Message;
using DistributeSystem.Contract.Abstractions.Shared;
using DistributeSystem.Contract.Services.V1.Identity;
using System.Security.Claims;

namespace DistributeSystem.Application.UserCases.V1.Commands.Identity;

public class RevokeTokenCommandHandler : ICommandHandler<Command.Revoke>
{
    private readonly ICacheService _cacheService;
    private readonly IJwtTokenService _jwtTokenService;

    public RevokeTokenCommandHandler(IJwtTokenService jwtTokenService, ICacheService cacheService)
    {
        _jwtTokenService = jwtTokenService;
        _cacheService = cacheService;
    }

    public async Task<Result> Handle(Command.Revoke request, CancellationToken cancellationToken)
    {
        var AccessToken = request.AccessToken;
        var principal = _jwtTokenService.GetPrincipalFromExpiredToken(AccessToken);
        var emailKey = principal.FindFirstValue(ClaimTypes.Email).ToString();

        var authenticated = await _cacheService.GetAsync<Response.Authenicated>(emailKey);

        if (authenticated == null)
        {
            throw  new Exception("Cannot get value from Redis");
        }

        await _cacheService.RemoveAsync(emailKey, cancellationToken);
        return Result.Success();
    }
}
