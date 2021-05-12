using System;
using System.IO;

namespace Niflheim.Installer.Services
{
    public class PreferencesService
    {
        public string GetPreferredNiflheimInstallPath(FileInfo preferenceFile)
        {
            if (!preferenceFile.Exists)
            {
                return @"C:\Program Files (x86)\Steam\steamapps\common\Niflheim";
            }
            else
            {
                try
                {
                    return File.ReadAllText(preferenceFile.FullName);
                }
                catch (Exception)
                {
                    return @"C:\Program Files (x86)\Steam\steamapps\common\Niflheim";
                }
            }
        }

        public void SetPreferredNiflheimInstallPath(FileInfo preferenceFile, string path)
        {
            File.WriteAllText(preferenceFile.FullName, path);
        }
    }
}
