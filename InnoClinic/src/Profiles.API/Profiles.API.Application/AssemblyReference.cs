using System.Reflection;

namespace InnoClinic.Profiles.API.Application
{
    public class AssemblyReference
    {
        public static readonly Assembly Assembly = typeof(AssemblyReference).Assembly;
    }
}
