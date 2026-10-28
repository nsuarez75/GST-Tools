// {l:T 'Texto'} en las vistas: el texto en castellano traducido al idioma de la interfaz (ver L.cs).
using Avalonia.Markup.Xaml;

namespace GstTools;

public sealed class TExtension(string text) : MarkupExtension
{
    public override object ProvideValue(IServiceProvider serviceProvider) => L.T(text);
}
