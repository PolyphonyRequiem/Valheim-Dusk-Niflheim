using System.Collections.Generic;
using System.IO;
using System.IO.Compression;


namespace ModManager
{
    public class ZipExtractor : ExtractorBase
    {

        public override void Extract(FileInfo archive, DirectoryInfo archiveTempRoot, DirectoryInfo componentRoot, List<FileMap> fileMaps)
        {
            string archivename = Path.GetFileNameWithoutExtension(archive.Name);
            DirectoryInfo archiveTempDirectory = new DirectoryInfo(Path.Combine(archiveTempRoot.FullName, archivename));
            if (archiveTempDirectory.Exists)
            {
                archiveTempDirectory.Delete(true);
            }

            ZipFile.ExtractToDirectory(archive.FullName, archiveTempDirectory.FullName);

            base.ArrangeFiles(archiveTempDirectory, componentRoot, fileMaps);
        }
    }
}