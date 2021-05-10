using Niflheim.Installer.Entities;
using Niflheim.Installer.Services;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace Niflheim.Core.Tests
{
    public class InstallerServiceTests
    {
        public InstallerServiceTests()
        {

        }

        [Fact]
        public void InstallModpackTest()
        {
            var x86 = Environment.GetFolderPath(Environment.SpecialFolder.ProgramFilesX86);
            var installer = new InstallerService(new DirectoryInfo(Path.Combine(x86, @"Steam\steamapps\common\Valheim")), new DirectoryInfo(Path.Combine(x86, @"Steam\steamapps\common\ValheimInstallerTest")));

            installer.InstallOrUpdate(new ModpackArchiveDefinition {ArchiveUri= new Uri("https://niflheim.blob.core.windows.net/modpacks/Niflheim-1.2.0-T4-2-debug.zip"), Version="test" });
        }
    }
}
