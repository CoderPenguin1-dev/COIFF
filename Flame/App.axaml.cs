using System.IO;
using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Markup.Xaml;
using Flame;

namespace Flame;

public partial class App : Application
{
    public override void Initialize()
    {
        AvaloniaXamlLoader.Load(this);
    }

    public override void OnFrameworkInitializationCompleted()
    {
        if (ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
        {
            var args = desktop.Args;
            string filePath = string.Empty;
            
            // Find the game file.
            if (args.Length > 0)
                filePath = args[0];
            else if (File.Exists("game.cyoa"))
                filePath = "game.cyoa";
            
            if (filePath == string.Empty)
                return;

            desktop.MainWindow = new MainWindow()
            {
                GameFilePath = filePath,
            };
        }

        base.OnFrameworkInitializationCompleted();
    }
}