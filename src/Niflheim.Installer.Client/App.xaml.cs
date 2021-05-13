using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Niflheim.Installer.Client.Configuration;
using Niflheim.Installer.Client.Repositories;
using Niflheim.Installer.Client.ViewModels;
using Niflheim.Installer.Clients;
using Niflheim.Installer.Entities;
using Niflheim.Installer.Services;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Windows;

namespace Niflheim.Installer.Client
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        private IHost host;

        private TimeSpan ShutdownTimeout = TimeSpan.FromSeconds(5);

        public App()
        {
            host = new HostBuilder()
                       .ConfigureAppConfiguration(ConfigureApplication)
                       .ConfigureServices(ConfigureServices)
                       .ConfigureLogging(ConfigureLogging)
                       .Build();
        }

        private void ConfigureApplication(HostBuilderContext context, IConfigurationBuilder configurationBuilder)
        {
            configurationBuilder.AddJsonFile(".\\appsettings.json", optional: false, reloadOnChange: false);
        }

        private void ConfigureServices(HostBuilderContext context, IServiceCollection serviceCollection)
        {
            AppConfig config = new AppConfig();
            context.Configuration.GetSection("Configuration").Bind(config);

            var discoveryUrl = new Uri(config.DiscoveryUrl);
            var discoveryClient = new DiscoveryClient(discoveryUrl);

            var feedurl = discoveryClient.GetFeedUrl();

            serviceCollection.AddSingleton<MainWindow>()
                             .AddSingleton<MainViewModel>()
                             .AddSingleton<WebModpackRepository>()
                             .AddSingleton<PreferencesService>(new PreferencesService(new FileInfo(@".\valheimlaucher.preferences.cfg"), new Dictionary<string, string>
                             {
                                 {"valheimPath", @"C:\Program Files (x86)\Steam\steamapps\common\Valheim" },
                                 {"niflheimPath", @"C:\Program Files (x86)\Steam\steamapps\common\Niflheim" },
                                 {"steamDetectionCompleted", "false" }
                             }))
                             .AddSingleton<JsonModpackClient<ModpackArchiveDefinition>>(new JsonModpackClient<ModpackArchiveDefinition>(feedurl))
                             .AddSingleton<DiscoveryClient>(discoveryClient)
                             .AddSingleton<AppConfig>(config);
        }

        private void ConfigureLogging(HostBuilderContext context, ILoggingBuilder loggingBuilder)
        {
            loggingBuilder.AddDebug();
        }

        private async void OnApplicationStartup(object sender, StartupEventArgs e)
        {
            await host.StartAsync();

            this.MainWindow = host.Services.GetRequiredService<MainWindow>();
            this.MainWindow.Show();
        }

        private async void OnApplicationExit(object sender, ExitEventArgs e)
        {
            await host.StopAsync(ShutdownTimeout);
            Debug.WriteLine("Host stopped.");
            Process.GetCurrentProcess().Kill();
        }
    }
}
