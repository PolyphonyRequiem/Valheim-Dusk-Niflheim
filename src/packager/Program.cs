using System;
using System.IO;
using System.Reflection;
using System.Threading.Tasks;

namespace Firoso.Niflheim.Tools.Packager
{
    class Program
    {
        static async Task Main(string[] args)
        {
            if (args.Length < 4)
            {
                var versionString = Assembly.GetEntryAssembly()
                                    .GetCustomAttribute<AssemblyInformationalVersionAttribute>()
                                    .InformationalVersion
                                    .ToString();

                Console.WriteLine($"niflheim-packager v{versionString}");
                Console.WriteLine("-------------");
                Console.WriteLine("\nUsage:");
                Console.WriteLine(@"  dotnet tool run modpack <ManifestFile> <NexusApiKey> <RelativeOutputPath> <DownloadedArchives>");
                return;
            }


            var manifestFile = new FileInfo(args[0]);
            var nexusApiKey = args[1];
            var outputPath = new DirectoryInfo(args[2]);
            var archivePath = new DirectoryInfo(args[3]);

            Console.WriteLine("-------------STARTING PACKAGE DOWNLOAD-------------");

            var resolver = new ModManager.ModManifestResolver(nexusApiKey);

            await resolver.ResolveManifest(manifestFile, outputPath, archivePath);
        }
    }
}
