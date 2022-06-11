using System.Text;

internal class MarkupBuilder
{
    private LinkedList<Markup> _list;

    internal MarkupBuilder() {
        _list = new LinkedList<Markup>();
    }

    internal MarkupBuilder(string text) : this() {
        Append(text);
    }

    public MarkupBuilder SetStyle(Style style) {
        _list.AddLast(new Markup.StyleMarker(style));
        return this;
    }

    public MarkupBuilder ResetStyle() {
        _list.AddLast(Markup.StyleMarker.Default);
        return this;
    }

    public MarkupBuilder SetTextFormat(TextFormat format) {
        _list.AddLast(new Markup.TextFormatMarker(format));
        return this;
    }

    public MarkupBuilder ResetTextFormat() {
        _list.AddLast(Markup.TextFormatMarker.Default);
        return this;
    }

    public MarkupBuilder SetForeground(TextColor color) {
        _list.AddLast(new Markup.ColorMarker(color, false));
        return this;
    }

    public MarkupBuilder ResetForeground() {
        _list.AddLast(Markup.ColorMarker.DefaultForeground);
        return this;
    }

    public MarkupBuilder SetBackground(TextColor color) {
        _list.AddLast(new Markup.ColorMarker(color, true));
        return this;
    }

    public MarkupBuilder ResetBackground() {
        _list.AddLast(Markup.ColorMarker.DefaultBackground);
        return this;
    }

    public MarkupBuilder Append(string text) {
        _list.AddLast(new Markup.Text(text));
        return this;
    }

    public MarkupBuilder Append(string text, Style style) {
        _list.AddLast(new Markup.StyleMarker(style));
        _list.AddLast(new Markup.Text(text));
        _list.AddLast(Markup.StyleMarker.Default);
        return this;
    }

    public MarkupBuilder Append(MarkupBuilder markups, Style style) {
        if (markups._list.First is not null) {
            _list.AddLast(new Markup.StyleMarker(style));
            _list.AddLast(markups._list.First);
            _list.AddLast(Markup.StyleMarker.Default);
        }

        return this;
    }

    public MarkupBuilder Append(string text, TextColor color, bool asBackground = false) {
        _list.AddLast(new Markup.ColorMarker(color, asBackground));
        _list.AddLast(new Markup.Text(text));
        _list.AddLast(asBackground ? Markup.ColorMarker.DefaultBackground : Markup.ColorMarker.DefaultForeground);
        return this;
    }

    public MarkupBuilder Append(MarkupBuilder markups, TextColor color, bool asBackground = false) {
        if (markups._list.First is not null) {
            _list.AddLast(new Markup.ColorMarker(color, asBackground));
            _list.AddLast(markups._list.First);
            _list.AddLast(asBackground ? Markup.ColorMarker.DefaultBackground : Markup.ColorMarker.DefaultForeground);
        }

        return this;
    }

    public MarkupBuilder Append(string text, TextFormat format) {
        _list.AddLast(new Markup.TextFormatMarker(format));
        _list.AddLast(new Markup.Text(text));
        _list.AddLast(Markup.TextFormatMarker.Default);
        return this;
    }

    public MarkupBuilder Append(MarkupBuilder markups, TextFormat format) {
        if (markups._list.First is not null) {
            _list.AddLast(new Markup.TextFormatMarker(format));
            _list.AddLast(markups._list.First);
            _list.AddLast(Markup.TextFormatMarker.Default);
        }

        return this;
    }

    public MarkupBuilder Append(char text) => Append(text.ToString());
    public MarkupBuilder Append(object obj) => Append(obj.ToString()!);

    public MarkupBuilder Append(char text, Style style) => Append(text.ToString(), style);
    public MarkupBuilder Append(object obj, Style style) => Append(obj.ToString()!, style);

    public MarkupBuilder Append(char text, TextColor color) => Append(text.ToString(), color);
    public MarkupBuilder Append(object obj, TextColor color) => Append(obj.ToString()!, color);

    public MarkupBuilder AppendLine() => Append("\n");
    public MarkupBuilder AppendLine(string text) => Append(text + '\n');
    public MarkupBuilder AppendLine(char text) => Append(text + '\n');
    public MarkupBuilder AppendLine(object obj) => Append(obj.ToString() + '\n');
    public MarkupBuilder AppendLine(MarkupBuilder markups) => Append(markups.AppendLine());

    public MarkupBuilder AppendLine(string text, Style style) => Append(text + '\n', style);
    public MarkupBuilder AppendLine(char text, Style style) => Append(text + '\n', style);
    public MarkupBuilder AppendLine(object obj, Style style) => Append(obj.ToString() + '\n', style);
    public MarkupBuilder AppendLine(MarkupBuilder markups, Style style) => Append(markups.AppendLine(), style);

    public MarkupBuilder AppendLine(string text, TextColor color) => Append(text + '\n', color);
    public MarkupBuilder AppendLine(char text, TextColor color) => Append(text + '\n', color);
    public MarkupBuilder AppendLine(object obj, TextColor color) => Append(obj.ToString() + '\n', color);
    public MarkupBuilder AppendLine(MarkupBuilder markups, TextColor color) => Append(markups.AppendLine(), color);

    public override string ToString() {
        var sb = new StringBuilder();

        foreach (var node in _list.OfType<Markup.Text>()) {
            sb.Append(node.Content);
        }

        return sb.ToString();
    }

    public string Render() {
        var sb = new StringBuilder();

        foreach (var node in _list) {
            sb.Append(node.ToString());
        }

        return sb.ToString();
    }
}