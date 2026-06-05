using System;
using System.IO;
using System.Threading.Tasks;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Input.Platform;
using Avalonia.Markup.Xaml;
using ReactiveUI;

namespace AlphaGrabApp;

public class App : Application
{
    internal static readonly string IconPath =
        Path.Combine(AppContext.BaseDirectory, "resources", "images", "icon.png");

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
                Icon = new WindowIcon(IconPath),
                ToolTipText = "Alpha Grab",
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
                return; // user cancelled — do nothing

            if (_trayIcon != null) _trayIcon.ToolTipText = "Extracting text…";

            try
            {
                var text = await TextExtractor.ExtractTextFromScreenshotAsync(screenshotPath);

                await Avalonia.Threading.Dispatcher.UIThread.InvokeAsync(async () =>
                {
                    var toast = new Toast(text);
                    toast.Show();

                    if (!string.IsNullOrEmpty(text))
                    {
                        var clipboard = TopLevel.GetTopLevel(toast)?.Clipboard;
                        if (clipboard != null)
                            await clipboard.SetTextAsync(text);
                    }
                });
            }
            catch (Exception ex)
            {
                var inner = ex;
                while (inner.InnerException != null) inner = inner.InnerException;
                await Avalonia.Threading.Dispatcher.UIThread.InvokeAsync(() =>
                {
                    var toast = new Toast($"Could not extract text: {inner.GetType().Name}: {inner.Message}");
                    toast.Show();
                });
            }
            finally
            {
                if (_trayIcon != null) _trayIcon.ToolTipText = "Alpha Grab";
                await Screenshotter.DeleteTempScreenshotAsync(screenshotPath);
            }
        };

        return grabItem;
    }
}
