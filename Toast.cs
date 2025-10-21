using Avalonia;
using Avalonia.Controls;
using Avalonia.Media;
using ReactiveUI;

public class Toast : Window
{
    public Toast(string text)
    {
        Width = 500;
        Height = 200;
        CanResize = false;
        SystemDecorations = SystemDecorations.None;
        TransparencyLevelHint = [WindowTransparencyLevel.AcrylicBlur, WindowTransparencyLevel.Transparent];
        Background = Brushes.BlueViolet;
        Topmost = true;

        Content = new StackPanel
        {
            Margin = new Thickness(10),
            Children =
            {
                new TextBlock { Text = text, TextWrapping = TextWrapping.Wrap, Margin = new Thickness(0,0,0,8) },
                new Button { Content = "Copy to Clipboard", Command = ReactiveCommand.CreateFromTask(async () =>
                {
                    await Avalonia.Threading.Dispatcher.UIThread.InvokeAsync(async () =>
                    {
                        var clipboard = GetTopLevel(this)?.Clipboard;
                        if (clipboard != null)
                            await clipboard.SetTextAsync(text);
                    });
                }) }
            }
        };

        var screen = Screens.Primary.Bounds;
        Position = new PixelPoint((int)(screen.Width - Width - 20), (int)(screen.Height - Height - 40));
    }
}
