using DistributeSystem.Contract.Abstractions.Message;

namespace DistributeSystem.Contract.Services.V1.Identity;

public static class Query
{
    public record Login(string Email, string Password) : IQuery<Response.Authenicated>;

    public record Token(string? AccessToken, string? RefreshToken) : IQuery<Response.Authenicated>;
}
