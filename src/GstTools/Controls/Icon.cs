using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Documents;
using Avalonia.Media;

namespace GstTools.Controls;

// Icono de línea (trazos de Lucide, licencia ISC, en una rejilla de 24×24), como en las demás aplicaciones.
// Se dibuja con el color de texto heredado, así sigue al tema y al estado del botón que lo contiene.
public sealed class Icon : Control
{
    public static readonly StyledProperty<string?> KindProperty = AvaloniaProperty.Register<Icon, string?>(nameof(Kind));

    public static readonly StyledProperty<IBrush?> ForegroundProperty = TextElement.ForegroundProperty.AddOwner<Icon>();

    private static readonly Dictionary<string, string> Paths = new(StringComparer.OrdinalIgnoreCase)
    {
        ["play"] = "M6 3l14 9-14 9z",
        ["download"] = "M21 15v4a2 2 0 0 1-2 2H5a2 2 0 0 1-2-2v-4 M7 10l5 5 5-5 M12 15V3",
        ["folder"] = "M6 14l1.5-2.9A2 2 0 0 1 9.24 10H20a2 2 0 0 1 1.94 2.5l-1.55 6a2 2 0 0 1-1.94 1.5H4a2 2 0 0 1-2-2V5a2 2 0 0 1 2-2h3.9a2 2 0 0 1 1.69.9l.81 1.2a2 2 0 0 0 1.67.9H18a2 2 0 0 1 2 2v2",
        ["alert"] = "M12 2a10 10 0 1 0 0 20a10 10 0 1 0 0-20z M12 8v4 M12 16h.01",
    };

    private static readonly Dictionary<string, Geometry> Cache = new(StringComparer.OrdinalIgnoreCase);

    static Icon()
    {
        AffectsRender<Icon>(KindProperty, ForegroundProperty);
    }

    public Icon()
    {
        Width = 16;
        Height = 16;
    }

    public string? Kind
    {
        get => GetValue(KindProperty);
        set => SetValue(KindProperty, value);
    }

    public IBrush? Foreground
    {
        get => GetValue(ForegroundProperty);
        set => SetValue(ForegroundProperty, value);
    }

    public override void Render(DrawingContext context)
    {
        if (Kind is not { } kind || !Paths.TryGetValue(kind, out var data))
        {
            return;
        }
        if (!Cache.TryGetValue(kind, out var geometry))
        {
            Cache[kind] = geometry = Geometry.Parse(data);
        }
        var scale = Math.Min(Bounds.Width, Bounds.Height) / 24;
        if (scale <= 0)
        {
            return;
        }
        var pen = new Pen(Foreground ?? Brushes.Gray, 2, lineCap: PenLineCap.Round, lineJoin: PenLineJoin.Round);
        var offset = new Vector((Bounds.Width - 24 * scale) / 2, (Bounds.Height - 24 * scale) / 2);
        using (context.PushTransform(Matrix.CreateScale(scale, scale) * Matrix.CreateTranslation(offset)))
        {
            context.DrawGeometry(null, pen, geometry);
        }
    }
}
