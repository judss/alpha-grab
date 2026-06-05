using System;
using System.Linq;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Layout;
using Avalonia.Media;
using Avalonia.Media.Imaging;
using Avalonia.Threading;

namespace AlphaGrabApp;

public class TextDetailWindow : Window
{
    private static readonly SolidColorBrush RowNormal = new(Color.FromArgb(0, 0, 0, 0));
    private static readonly SolidColorBrush RowHover = new(Color.Parse("#2A2A3E"));
    private static readonly SolidColorBrush RowCopied = new(Color.Parse("#1A3D2B"));

    public TextDetailWindow(string text)
    {
        Title = "Alpha Grab";
        Width = 580;
        Height = 480;
        MinWidth = 400;
        MinHeight = 300;
        CanResize = true;
        SystemDecorations = SystemDecorations.Full;
        Icon = new WindowIcon(App.IconPath);

        // Header
        var header = new StackPanel
        {
            Orientation = Orientation.Horizontal,
            Margin = new Thickness(20, 16, 20, 8),
            Spacing = 10,
            Children =
            {
                new Image
                {
                    Width = 24,
                    Height = 24,
                    Source = new Bitmap(App.IconPath),
                    VerticalAlignment = VerticalAlignment.Center,
                },
                new StackPanel
                {
                    Children =
                    {
                        new TextBlock
                        {
                            Text = "Alpha Grab",
                            FontWeight = FontWeight.Bold,
                            FontSize = 15,
                        },
                        new TextBlock
                        {
                            Text = "Click a section to copy it",
                            FontSize = 11,
                            Foreground = new SolidColorBrush(Color.Parse("#707080")),
                        },
                    }
                }
            }
        };

        // Section rows
        var sectionsPanel = new StackPanel { Spacing = 4, Margin = new Thickness(12, 0, 12, 0) };

        var paragraphs = text
            .Split('\n', StringSplitOptions.RemoveEmptyEntries)
            .Select(p => p.Trim())
            .Where(p => p.Length > 0)
            .ToList();

        foreach (var paragraph in paragraphs)
        {
            var row = BuildSectionRow(paragraph);
            sectionsPanel.Children.Add(row);
        }

        var scrollViewer = new ScrollViewer
        {
            Content = sectionsPanel,
            VerticalScrollBarVisibility = Avalonia.Controls.Primitives.ScrollBarVisibility.Auto,
            Margin = new Thickness(0, 4, 0, 0),
        };

        // Footer
        var footer = new TextBlock
        {
            Text = "All text copied to clipboard",
            FontSize = 11,
            Foreground = new SolidColorBrush(Color.Parse("#707080")),
            HorizontalAlignment = HorizontalAlignment.Center,
            Margin = new Thickness(0, 8, 0, 12),
        };

        var separator = new Avalonia.Controls.Separator
        {
            Margin = new Thickness(20, 0),
            Background = new SolidColorBrush(Color.Parse("#30303A")),
        };

        Content = new DockPanel
        {
            Children =
            {
                header,
                footer,
                separator,
                scrollViewer,
            }
        };
        DockPanel.SetDock(header, Dock.Top);
        DockPanel.SetDock(footer, Dock.Bottom);
        DockPanel.SetDock(separator, Dock.Bottom);

        // Center on screen
        WindowStartupLocation = WindowStartupLocation.CenterScreen;
    }

    private Border BuildSectionRow(string paragraph)
    {
        var label = new TextBlock
        {
            Text = paragraph,
            TextWrapping = TextWrapping.Wrap,
            FontSize = 13,
            Margin = new Thickness(12, 8, 12, 8),
        };

        var row = new Border
        {
            CornerRadius = new CornerRadius(6),
            Background = RowNormal,
            Child = label,
            Cursor = new Avalonia.Input.Cursor(Avalonia.Input.StandardCursorType.Hand),
        };

        row.PointerEntered += (_, _) =>
        {
            if (row.Background != RowCopied)
                row.Background = RowHover;
        };

        row.PointerExited += (_, _) =>
        {
            if (row.Background != RowCopied)
                row.Background = RowNormal;
        };

        row.PointerPressed += async (_, _) =>
        {
            row.Background = RowCopied;
            label.Text = "✓ Copied!";

            var clipboard = TopLevel.GetTopLevel(this)?.Clipboard;
            if (clipboard != null)
                await clipboard.SetTextAsync(paragraph);

            var timer = new DispatcherTimer { Interval = TimeSpan.FromMilliseconds(800) };
            timer.Tick += (_, _) =>
            {
                timer.Stop();
                row.Background = RowNormal;
                label.Text = paragraph;
            };
            timer.Start();
        };

        return row;
    }
}
