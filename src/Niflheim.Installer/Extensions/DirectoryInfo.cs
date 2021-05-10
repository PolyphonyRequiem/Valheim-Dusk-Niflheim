using System.IO;

namespace Niflheim.Installer.Extensions
{
    public static class DirectoryInfoExtensions
    {
        public static void Copy(this System.IO.DirectoryInfo source, System.IO.DirectoryInfo destination, bool copySubDirs)
        {
            if (!source.Exists)
            {
                throw new DirectoryNotFoundException(
                    "Source directory does not exist or could not be found: "
                    + source);
            }

            System.IO.DirectoryInfo[] dirs = source.GetDirectories();

            // If the destination directory doesn't exist, create it.       
            Directory.CreateDirectory(destination.FullName);

            // Get the files in the directory and copy them to the new location.
            FileInfo[] files = source.GetFiles();
            foreach (FileInfo file in files)
            {
                string tempPath = Path.Combine(destination.FullName, file.Name);
                file.CopyTo(tempPath, false);
            }

            // If copying subdirectories, copy them and their contents to new location.
            if (copySubDirs)
            {
                foreach (System.IO.DirectoryInfo subdir in dirs)
                {
                    string tempPath = Path.Combine(destination.FullName, subdir.Name);
                    subdir.Copy(new System.IO.DirectoryInfo(tempPath), copySubDirs);
                }
            }
        }
    }
}
