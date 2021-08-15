using FoodTuner.Configuration;
using FoodTuner.FileHandlers;
using FoodTuner.Services;
using FoodTuner.ViewModels;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;

namespace FoodTuner
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        public App()
        {
            Services = ConfigureServices();
            this.InitializeComponent();
        }

        /// <summary>
        /// Gets the current <see cref="App"/> instance in use
        /// </summary>
        public new static App Current => (App)Application.Current;

        /// <summary>
        /// Gets the <see cref="IServiceProvider"/> instance to resolve application services.
        /// </summary>
        public IServiceProvider Services { get; }

        /// <summary>
        /// Configures the services for the application.
        /// </summary>
        private static IServiceProvider ConfigureServices()
        {
            ConfigurationBuilder configBuilder = new ConfigurationBuilder();
            configBuilder.AddJsonFile("appconfig.json");
            var config = configBuilder.Build();                   

            var services = new ServiceCollection();
            services.AddSingleton<WorkspaceViewModel>();
            services.AddSingleton<WorkspaceService>();
            services.AddSingleton(new EnduranceFileHandler(new FileInfo(config["enduranceFilePath"])));
            services.AddSingleton(new FoodRebalanceFileHandler(new DirectoryInfo(config["foodRebalanceDirectoryPath"])));
            return services.BuildServiceProvider();
        }
    }
}
