using System.Collections.Generic;
using System.IO;
using Microsoft.Extensions.FileSystemGlobbing;

namespace ModManager
{
    public abstract class ExtractorBase
    {
        public abstract void Extract(FileInfo archive, DirectoryInfo archiveTempDirectory, DirectoryInfo componentRoot, List<FileMap> fileMaps);

        protected virtual void ArrangeFiles(DirectoryInfo extractedArchiveRoot, DirectoryInfo componentRoot, List<FileMap> fileMaps)
        {
            fileMaps = fileMaps ?? new List<FileMap> { new FileMap() };
            foreach (var map in fileMaps)
            {
                Matcher matcher = new Matcher(System.StringComparison.InvariantCultureIgnoreCase);
                
                var includes = map.Sources.Split(';', System.StringSplitOptions.RemoveEmptyEntries);
                matcher.AddIncludePatterns(includes); 
                
                var excludes = map.Exclusions.Split(';', System.StringSplitOptions.RemoveEmptyEntries);
                matcher.AddExcludePatterns(excludes);
                var normalizedMatchRoot = Path.Combine(extractedArchiveRoot.FullName, map.SourceRoot);
                var result = matcher.GetResultsInFullPath(normalizedMatchRoot);

                foreach (var filename in result)
                {
                    var relativeFilePath = Path.GetRelativePath(normalizedMatchRoot, filename);
                    var destinationFile = Path.Combine(componentRoot.FullName, map.Destination, relativeFilePath);
                    var destinationPath = new DirectoryInfo(Path.GetDirectoryName(destinationFile)!);
                    if (!destinationPath.Exists)
                    {
                        destinationPath.Create();
                    }

                    File.Copy(filename, destinationFile, overwrite: true);
                }
            }
        }
    }
}