using Avalonia;
using Avalonia.Controls;
using Avalonia.Media;

public class Toast : Window
{
    private string _text;
    public Toast(string text)
    {
        _text = text;
        Width = 500;
        Height = 200;
        Cursor = new Avalonia.Input.Cursor(Avalonia.Input.StandardCursorType.Hand);
        CanResize = false;
        SystemDecorations = SystemDecorations.Full;
        TransparencyLevelHint = [WindowTransparencyLevel.AcrylicBlur, WindowTransparencyLevel.Transparent];
        Background = Brushes.BlueViolet;
        Topmost = true;
        Icon = new WindowIcon("resources/images/icon.png");
        Title = "Alpha Grab";

        PointerPressed += OnWindowClick;

        Content = new StackPanel
        {
            Margin = new Thickness(10),
            Children =
            {
                new TextBlock { Text = "Extracted Text", FontWeight = FontWeight.Bold, HorizontalAlignment = Avalonia.Layout.HorizontalAlignment.Stretch },
                new TextBlock { Text = _text, TextWrapping = TextWrapping.Wrap, Margin = new Thickness(0,0,0,8) },
            }
        };
        var screen = Screens.Primary.Bounds;
        Position = new PixelPoint((int)(screen.Width - Width - 20), (int)(screen.Height - Height - 40));
    }
    
    private void OnWindowClick(object? sender, Avalonia.Input.PointerPressedEventArgs e)
    {
        var width = 800;
        var height = 200;
        var inputWindow = new Window()
        {
            Width = width,
            Height = height,
            Title = "Alpha Grab",
            Icon = new WindowIcon("resources/images/icon.png"),
            Background = Brushes.BlueViolet,
            Content = new TextBox
            {
                Text = _text,
                Width = width,
                Height = height - 100,
                Background = Brushes.BlueViolet,
            }
        };
        inputWindow.Show();

        this.Close();
    }
}
