using System.Threading.Tasks;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Markup.Xaml;
using ReactiveUI;

namespace AlphaGrabApp;

public class App : Application
{
    private TrayIcon? _trayIcon;

    public override void Initialize()
    {
        AvaloniaXamlLoader.Load(this);
    }

    public override void OnFrameworkInitializationCompleted()
    {
        if (ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
        {
            desktop.ShutdownMode = ShutdownMode.OnExplicitShutdown;

            _trayIcon = new TrayIcon
            {
                Icon = new WindowIcon("Assets/icon.png"),
                Menu = new NativeMenu
                {
                    Items =
                    {
                        GetGrabTextMenuItem(),
                        new NativeMenuItemSeparator(),
                        new NativeMenuItem { Header = "Quit", Command = ReactiveCommand.Create(() => desktop.Shutdown()) }
                    }
                }
            };
            _trayIcon.IsVisible = true;
        }

        base.OnFrameworkInitializationCompleted();
    }

    private NativeMenuItem GetGrabTextMenuItem()
    {
        var grabItem = new NativeMenuItem { Header = "Grab Text" };
        grabItem.Click += async (_, __) =>
        {
            var screenshotPath = Screenshotter.CaptureInteractiveToTempFile();

            if (screenshotPath == null)
                return;

            var text = await TextExtractor.ExtractTextFromScreenshotAsync(screenshotPath);

            if (text != null)
            {
                await Avalonia.Threading.Dispatcher.UIThread.InvokeAsync(async () =>
                {
                    var toast = new Toast(text);
                    toast.Show();
                });
            }

            await Screenshotter.DeleteTempScreenshotAsync(screenshotPath);
        };
        
        return grabItem;
    }
}
