using System.Reflection;

namespace DistributeSystem.Infrastructure;
public static class AssemblyReference
{
    public static readonly Assembly Assembly = typeof(AssemblyReference).Assembly;
}