using Niflheim.Installer.Entities;
using Niflheim.Installer.Services;
using Prism.Commands;
using System;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Threading.Tasks;

namespace Niflheim.Installer.Client.Commands
{
    public class InstallCommand : DelegateCommand, INotifyPropertyChanged
    {
        private string niflheimPath = "";
        private string valheimPath = "";
        private ModpackArchiveDefinition modpack;

        private readonly IProgress<string> progress;
        private readonly string serverEndpoint;

        public event PropertyChangedEventHandler PropertyChanged;

        public InstallCommand(IProgress<string> progress, string serverEndpoint) : base(() => { }, () => false)
        {
            this.progress = progress;
            this.serverEndpoint = serverEndpoint;
        }

        protected override void Execute(object parameter)
        {
            Task.Run(() =>
            this.Mode switch
            {
                InstallCommandMode.Checking => throw new InvalidOperationException(),
                InstallCommandMode.Install => this.Install(),
                InstallCommandMode.Update => this.Update(),
                InstallCommandMode.Launch => this.Launch(),
                _ => throw new NotImplementedException(),
            });
        }

        private bool Launch()
        {
            FileInfo niflheimExe = new FileInfo($"{Path.Combine(niflheimPath, "valheim.exe")}");
            if (!niflheimExe.Exists)
            {
                progress.Report($"Valheim.exe not found at {niflheimExe.FullName}");
                return false;
            }

            progress.Report($"Launching Niflheim.");
            var process = Process.Start(niflheimExe.FullName, $"+connect {serverEndpoint}");

            process.WaitForExit();

            return true;
        }

        private bool Update()
        {
            try
            {
                var installer = new InstallerService(new DirectoryInfo(this.valheimPath), new DirectoryInfo(this.niflheimPath));
                progress.Report($"Updating to {modpack.Version}... Please wait!");
                installer.Update(this.modpack);
                progress.Report($"Update installed!  Ready to Launch!");
                this.SetLaunch(this.niflheimPath);
                return true;
            }
            catch (Exception exc)
            {
                progress.Report($"AN ERROR OCCURRED: {exc.Message}");
                this.SetInstall(valheimPath, niflheimPath, modpack);
                return false;
            }

        }

        private bool Install()
        {
            try
            {

                var installer = new InstallerService(new DirectoryInfo(this.valheimPath), new DirectoryInfo(this.niflheimPath));
                progress.Report($"Installing Niflheim to {this.niflheimPath} and updating to {modpack.Version}... Please wait!");
                installer.CleanInstall(this.modpack);
                progress.Report($"Update installed!  Ready to Launch!");
                this.SetLaunch(this.niflheimPath);
                return true;
            }
            catch (Exception exc)
            {
                progress.Report($"AN ERROR OCCURRED: {exc.Message}");
                this.SetInstall(valheimPath, niflheimPath, modpack);
                return false;
            }
        }

        private InstallCommandMode mode = InstallCommandMode.Checking;

        private InstallCommandMode Mode
        {
            get { return mode; }
            set
            {
                this.mode = value;
                base.RaiseCanExecuteChanged();
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(InstallText)));
            }
        }

        private enum InstallCommandMode
        {
            Checking,
            Install,
            Update,
            Launch,
            Working
        }

        public string InstallText => this.Mode switch
        {
            InstallCommandMode.Checking => "Checking...",
            InstallCommandMode.Install => "Install",
            InstallCommandMode.Update => "Update",
            InstallCommandMode.Launch => "Launch",
            InstallCommandMode.Working => "Working...",
            _ => ""
        };

        public void SetLaunch(string niflheimPath)
        {
            this.Mode = InstallCommandMode.Launch;
            this.niflheimPath = niflheimPath;
        }

        public void SetInstall(string valheimPath, string niflheimPath, ModpackArchiveDefinition modpack)
        {
            this.Mode = InstallCommandMode.Install;
            this.modpack = modpack;
            this.niflheimPath = niflheimPath;
            this.valheimPath = valheimPath;
        }

        public void SetUpdate(string niflheimPath, ModpackArchiveDefinition modpack)
        {
            this.modpack = modpack;
            this.Mode = InstallCommandMode.Update;
            this.niflheimPath = niflheimPath;
        }

        public void SetWorking()
        {
            this.Mode = InstallCommandMode.Working;
        }

        protected override bool CanExecute(object parameter)
        {
            return this.Mode switch
            {
                InstallCommandMode.Checking => false,
                InstallCommandMode.Install => true,
                InstallCommandMode.Update => true,
                InstallCommandMode.Launch => true,
                InstallCommandMode.Working => false,
                _ => false
            };
        }
    }
}
