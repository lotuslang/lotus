internal abstract record Markup
{
    internal abstract string DbgString();

    [DebuggerStepThrough]
    [DebuggerDisplay("{DbgString()}")]
    public sealed record TextFormatMarker(TextFormat Format) : Markup
    {
        public static readonly TextFormatMarker None =
            new TextFormatMarker(TextFormat.None);

        public static readonly TextFormatMarker Reset =
            new TextFormatMarker(TextFormat.Reset);

        public override string ToString()
            => MarkupUtils.ToString(Format);

        internal override string DbgString()
            => $"FMT({Format})";
    }

    [DebuggerStepThrough]
    [DebuggerDisplay("{DbgString()}")]
    public sealed record ColorMarker(TextColor Color, bool IsBackground) : Markup
    {
        public static readonly ColorMarker ResetBackground =
            new ColorMarker(TextColor.ResetColor, true);
        public static readonly ColorMarker ResetForeground =
            new ColorMarker(TextColor.ResetColor, false);

        public override string ToString()
            => "\x1b[" + (IsBackground ? Color.GetBGString() : Color.GetFGString()) + 'm';

        internal override string DbgString()
            => $"{(IsBackground ? "BG" : "FG")}({Color.GetType().Name})";
    }

    [DebuggerStepThrough]
    [DebuggerDisplay("{DbgString()}")]
    public sealed record StyleMarker(Style Style) : Markup
    {
        public static readonly StyleMarker Reset =
            new StyleMarker(
                new Style(
                    TextColor.ResetColor,
                    TextColor.ResetColor,
                    TextFormat.Reset));

        public override string ToString()
            => Style.ToString();

        internal override string DbgString()
            => "STY(" + Style.DbgString() + ")";
    }

    [DebuggerStepThrough]
    public sealed record Text(string Content) : Markup
    {
        public override string ToString() => Content;

        internal override string DbgString() => Content;
    }
}