using FoodTuner.ViewModels;
using System.Windows;
using Microsoft.Extensions.DependencyInjection;

namespace FoodTuner
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            this.DataContext = App.Current.Services.GetRequiredService<WorkspaceViewModel>();
            InitializeComponent();
        }
    }
}
