using System.IO;
using System.IO.Compression;

namespace Niflheim.Installer.Services
{
    public static class ArchiveExtractor
    {
        public static void ExtractArchive(FileInfo archiveFileInfo, System.IO.DirectoryInfo destinationDirectoryInfo)
        {
            using (FileStream archiveFileStream = File.OpenRead(archiveFileInfo.FullName))
            using (ZipArchive archive = new ZipArchive(archiveFileStream))
            {
                archive.ExtractToDirectory(destinationDirectoryInfo.FullName, true);
            }
        }
    }
}
