internal abstract record Markup
{
    [System.Diagnostics.DebuggerStepThrough]
    public record TextFormatMarker(TextFormat Format) : Markup
    {
        public static readonly TextFormatMarker Default =
            new TextFormatMarker(TextFormat.None);

        public static readonly TextFormatMarker Reset =
            new TextFormatMarker(TextFormat.Reset);

        public override string ToString()
            => MarkupUtils.ToString(Format);
    }

    [System.Diagnostics.DebuggerStepThrough]
    public record ColorMarker(TextColor Color, bool IsBackground) : Markup
    {
        public static readonly ColorMarker DefaultBackground =
            new ColorMarker(TextColor.DefaultColor, true);
        public static readonly ColorMarker DefaultForeground =
            new ColorMarker(TextColor.DefaultColor, false);

        public override string ToString()
            => IsBackground ? Color.GetBGString() : Color.GetFGString();
    }

    [System.Diagnostics.DebuggerStepThrough]
    public record StyleMarker(Style Style) : Markup
    {
        public static readonly StyleMarker Default =
            new StyleMarker(
                new Style(
                    TextColor.DefaultColor,
                    TextColor.DefaultColor,
                    TextFormat.None));

        public override string ToString()
            => Style.ToString();
    }

    [System.Diagnostics.DebuggerStepThrough]
    public record Text(string Content) : Markup
    {
        public override string ToString() => Content;
    }
}