using System.Collections.Generic;
using System.IO;


namespace ModManager
{
    public class NoExtractor : ExtractorBase
    {
        public override void Extract(FileInfo archive, DirectoryInfo archiveTempRoot, DirectoryInfo componentRoot, List<FileMap> fileMaps)
        {
            // lol, nvm, it's just a single file! How much damage could it do?!
            string archivename = Path.GetFileNameWithoutExtension(archive.Name);
            DirectoryInfo archiveTempDirectory = new DirectoryInfo(Path.Combine(archiveTempRoot.FullName, archivename));
            if (archiveTempDirectory.Exists)
            {
                archiveTempDirectory.Delete(true);
            }
            DirectoryInfo destinationDirectory = new DirectoryInfo(Path.Join(componentRoot.FullName, fileMaps[0].Destination));

            if (!destinationDirectory.Exists)
            {
                destinationDirectory.Create();
            }

            File.Copy(archive.FullName, Path.Join(componentRoot.FullName, fileMaps[0].Destination, fileMaps[0].Sources));
        }
    }
}