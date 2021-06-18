using SharpCompress.Common;
using SharpCompress.Readers;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;


namespace ModManager
{
    public class RarExtractor : ExtractorBase
    {
        public override void Extract(FileInfo archive, DirectoryInfo archiveTempRoot, DirectoryInfo componentRoot, List<FileMap> fileMaps)
        {
            string archivename = Path.GetFileNameWithoutExtension(archive.Name);
            DirectoryInfo archiveTempDirectory = new DirectoryInfo(Path.Combine(archiveTempRoot.FullName, archivename));
            if (archiveTempDirectory.Exists)
            {
                archiveTempDirectory.Delete(true);
            }

            using (Stream stream = File.OpenRead(archive.FullName))
            {
                var reader = ReaderFactory.Open(stream);
                while (reader.MoveToNextEntry())
                {
                    if (!reader.Entry.IsDirectory)
                    {
                        reader.WriteEntryToDirectory(archiveTempDirectory.FullName, new ExtractionOptions() { ExtractFullPath = true, Overwrite = true });
                    }
                }
            }

            base.ArrangeFiles(archiveTempDirectory, componentRoot, fileMaps);
        }
    }
}