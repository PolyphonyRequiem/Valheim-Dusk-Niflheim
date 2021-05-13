using Ookii.Dialogs.Wpf;
using Prism.Commands;
using System;

namespace Niflheim.Installer.Client.Commands
{
    public class BrowseCommand : DelegateCommand
    {
        public BrowseCommand(Func<string> pathGetter, Action<string> pathHandler) : base(() =>
        {
            var dialog = new VistaFolderBrowserDialog();
            dialog.ShowNewFolderButton = true;
            dialog.SelectedPath = pathGetter.Invoke();
            if (dialog.ShowDialog() ?? false)
            {
                pathHandler(dialog.SelectedPath);
            }
        })
        { }

    }
}
