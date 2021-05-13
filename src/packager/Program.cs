using System;
using System.Reflection;

namespace Firoso.Niflheim.Tools.Packager
{
    class Program
    {
        static void Main(string[] args)
        {
            var versionString = Assembly.GetEntryAssembly()
                                .GetCustomAttribute<AssemblyInformationalVersionAttribute>()
                                .InformationalVersion
                                .ToString();

            Console.WriteLine($"niflheim-packager v{versionString}");
            Console.WriteLine("-------------");
            Console.WriteLine("\nUsage:");
            Console.WriteLine(@"  dotnet tool niflheim-packager package.manifest.json -o ./output");
            return;
        }
    }
}
