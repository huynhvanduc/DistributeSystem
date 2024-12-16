using DistributeSystem.Contract.Abstractions.Message;

namespace DistributeSystem.Contract.Services.V1.Identity;

public static class Command
{
    public record Revoke(string AccessToken) : ICommand;
}
