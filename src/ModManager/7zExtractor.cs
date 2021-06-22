using SharpCompress.Archives;
using SharpCompress.Common;
using SharpCompress.Readers;
using System.Collections.Generic;
using System.IO;

namespace ModManager
{
    public class SevenZipExtractor : ExtractorBase
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
                var archiveReader = ArchiveFactory.Open(archive.FullName).ExtractAllEntries();

                while (archiveReader.MoveToNextEntry())
                {
                    if (!archiveReader.Entry.IsDirectory)
                    {
                        archiveReader.WriteEntryToDirectory(archiveTempDirectory.FullName, new ExtractionOptions() { ExtractFullPath = true, Overwrite = true });
                    }
                }
            }

            base.ArrangeFiles(archiveTempDirectory, componentRoot, fileMaps);
        }
    }
}