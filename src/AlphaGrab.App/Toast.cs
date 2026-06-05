using System;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Shapes;
using Avalonia.Layout;
using Avalonia.Media;
using Avalonia.Media.Imaging;
using Avalonia.Threading;

namespace AlphaGrabApp;

public class Toast : Window
{
    private readonly string _text;

    public Toast(string text)
    {
        _text = text;

        Width = 380;
        Height = 90;
        CanResize = false;
        SystemDecorations = SystemDecorations.None;
        TransparencyLevelHint = [WindowTransparencyLevel.AcrylicBlur, WindowTransparencyLevel.Transparent];
        Background = new SolidColorBrush(Color.FromArgb(235, 30, 30, 46)); // #1E1E2E
        Topmost = true;
        ShowInTaskbar = false;

        var preview = string.IsNullOrWhiteSpace(text)
            ? "No text detected"
            : text.Split('\n', StringSplitOptions.RemoveEmptyEntries)[0].Trim();

        var icon = new Image
        {
            Width = 36,
            Height = 36,
            Source = new Bitmap(App.IconPath),
            VerticalAlignment = VerticalAlignment.Center,
            Margin = new Thickness(16, 0, 12, 0),
        };

        var textStack = new StackPanel
        {
            VerticalAlignment = VerticalAlignment.Center,
            Children =
            {
                new TextBlock
                {
                    Text = string.IsNullOrWhiteSpace(text) ? "Nothing captured" : "Text grabbed",
                    FontWeight = FontWeight.SemiBold,
                    FontSize = 13,
                    Foreground = new SolidColorBrush(Color.Parse("#E0E0E0")),
                },
                new TextBlock
                {
                    Text = preview,
                    FontSize = 12,
                    Foreground = new SolidColorBrush(Color.Parse("#9090A0")),
                    TextTrimming = TextTrimming.CharacterEllipsis,
                    MaxWidth = 270,
                },
                new TextBlock
                {
                    Text = string.IsNullOrWhiteSpace(text) ? "" : "Copied to clipboard · Click to select sections",
                    FontSize = 10,
                    Foreground = new SolidColorBrush(Color.Parse("#6060A0")),
                    Margin = new Thickness(0, 2, 0, 0),
                },
            }
        };

        var arrow = new TextBlock
        {
            Text = "›",
            FontSize = 20,
            Foreground = new SolidColorBrush(Color.Parse("#6060A0")),
            VerticalAlignment = VerticalAlignment.Center,
            Margin = new Thickness(8, 0, 14, 0),
        };

        var layout = new Border
        {
            CornerRadius = new CornerRadius(12),
            ClipToBounds = true,
            Child = new DockPanel
            {
                LastChildFill = true,
                Children =
                {
                    icon,
                    arrow,
                    textStack,
                }
            }
        };
        DockPanel.SetDock(icon, Dock.Left);
        DockPanel.SetDock(arrow, Dock.Right);

        Content = layout;
        Cursor = new Avalonia.Input.Cursor(Avalonia.Input.StandardCursorType.Hand);

        PointerPressed += (_, _) =>
        {
            if (!string.IsNullOrWhiteSpace(_text))
            {
                var detail = new TextDetailWindow(_text);
                detail.Show();
            }
            Close();
        };

        // Position bottom-right
        Opened += (_, _) =>
        {
            var screen = Screens.Primary?.Bounds;
            if (screen != null)
                Position = new PixelPoint(
                    (int)(screen.Value.Width - Width - 20),
                    (int)(screen.Value.Height - Height - 40));
        };

        // Auto-dismiss after 5 seconds
        var timer = new DispatcherTimer { Interval = TimeSpan.FromSeconds(5) };
        timer.Tick += (_, _) => { timer.Stop(); Close(); };
        timer.Start();
    }
}
