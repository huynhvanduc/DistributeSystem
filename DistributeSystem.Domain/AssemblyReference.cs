using System.Reflection;

namespace DistributeSystem.Domain;
public class AssemblyReference
{
    public static readonly Assembly Assembly = typeof(AssemblyReference).Assembly;
}