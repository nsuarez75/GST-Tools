// Ventana «Acerca de»: nombre, versión, autor y enlace al repositorio, con un botón Aceptar abajo a la derecha.
using Avalonia;
using Avalonia.Controls;
using Avalonia.Layout;
using Avalonia.Media;

namespace GstTools;

public sealed class AboutWindow : Window
{
    public const string RepositoryUrl = "https://github.com/nsuarez75/GST-Tools";

    public static Version CurrentVersion => typeof(AboutWindow).Assembly.GetName().Version ?? new Version(0, 0, 0);

    public static string Text() =>
        L.T("{0}, versión {1}.\nCreado por Ignacio Suarez (Alomatek).", "GST Tools", CurrentVersion.ToString(3));

    public AboutWindow()
    {
        Title = L.T("Acerca de");
        Width = 480;
        SizeToContent = SizeToContent.Height;
        CanResize = false;
        ShowInTaskbar = false;
        WindowStartupLocation = WindowStartupLocation.CenterOwner;

        var body = new StackPanel { Spacing = 8 };
        body.Children.Add(new TextBlock { Text = Text(), TextWrapping = TextWrapping.Wrap });
        body.Children.Add(new HyperlinkButton { Content = RepositoryUrl, NavigateUri = new Uri(RepositoryUrl), Padding = new Thickness(0) });

        var accept = new Button
        {
            Content = L.T("Aceptar"), IsDefault = true, IsCancel = true, MinWidth = 90,
            HorizontalAlignment = HorizontalAlignment.Right, HorizontalContentAlignment = HorizontalAlignment.Center,
            Classes = { "accent" },
        };
        accept.Click += (_, _) => Close();

        var grid = new Grid { RowDefinitions = new RowDefinitions("Auto,*,Auto"), Margin = new Thickness(20), RowSpacing = 14 };
        grid.Children.Add(new TextBlock { Text = Title, Classes = { "h2" } });
        Grid.SetRow(body, 1);
        grid.Children.Add(body);
        Grid.SetRow(accept, 2);
        grid.Children.Add(accept);
        Content = grid;
    }
}
