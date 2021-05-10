using Niflheim.Installer.Entities;
using Niflheim.Installer.Services;
using System;
using System.IO;
using Xunit;

namespace Niflheim.Core.Tests
{
    public class ModpackArchiveDownloaderTests : IDisposable
    {
        private readonly DirectoryInfo downloadDirectory = new DirectoryInfo("./TestOutput/");

        private ModpackArchiveDefinition modpack = new ModpackArchiveDefinition
        {
            Id = "testModpack",
            Description = "This is a test modpack",
            Version = "0.0.0",
            Tag = "test",
            Disabled = false,
            ArchiveUri = new Uri("https://niflheim.blob.core.windows.net/modpacks/testmodpack.zip")
        };

        public ModpackArchiveDownloaderTests()
        {
            downloadDirectory.Create();
        }

        [Fact]
        public void GetReleaseArchiveNoOverload()
        {
            var result = ModpackArchiveDownloader.GetReleaseArchiveAsync(modpack).Result;
            Assert.True(result.Exists);
            result.Delete();
        }

        [Fact]
        public void GetReleaseArchiveWithOverload()
        {
            downloadDirectory.Create();
            var expectedArchiveFile = new FileInfo(Path.Join(downloadDirectory.FullName, "testArchive.zip"));
            var result = ModpackArchiveDownloader.GetReleaseArchiveAsync(modpack, expectedArchiveFile).Result;
            Assert.True(result.Exists);
            Assert.True(expectedArchiveFile.Exists);
            Assert.Equal(result, expectedArchiveFile);

            var expectedExtractedFile = new FileInfo(Path.Join(downloadDirectory.FullName, "testmodpack.txt"));
            ArchiveExtractor.ExtractArchive(result, downloadDirectory);

            Assert.True(expectedExtractedFile.Exists);

            result.Delete();
        }


        public void Dispose()
        {
            downloadDirectory.Delete(true);
        }
    }
}
