using System.Reflection;

namespace DistributeSystem.Persistence;
public static class AssemblyReference
{
    public static readonly Assembly Assembly = typeof(AssemblyReference).Assembly;
}