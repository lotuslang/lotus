namespace Lotus.Text;

internal abstract record Markup
{
    internal abstract string DbgStr();

    [DebuggerStepThrough]
    [DebuggerDisplay("{DbgStr(),nq}")]
    public sealed record TextFormatMarker(TextFormat Format) : Markup
    {
        public static readonly TextFormatMarker None =
            new(TextFormat.None);

        public static readonly TextFormatMarker Reset =
            new(TextFormat.Reset);

        public override string ToString()
            => MarkupUtils.ToString(Format);

        internal override string DbgStr()
            => $"FMT({Format})";
    }

    [DebuggerStepThrough]
    [DebuggerDisplay("{DbgStr(),nq}")]
    public sealed record ColorMarker(TextColor Color, bool IsBackground) : Markup
    {
        public static readonly ColorMarker ResetBackground =
            new(TextColor.Reset, true);
        public static readonly ColorMarker ResetForeground =
            new(TextColor.Reset, false);

        public override string ToString()
            => "\x1b[" + (IsBackground ? Color.GetBGString() : Color.GetFGString()) + 'm';

        internal override string DbgStr()
            => $"{(IsBackground ? "BG" : "FG")}({Color.GetType().Name})";
    }

    [DebuggerStepThrough]
    [DebuggerDisplay("{DbgStr(),nq}")]
    public sealed record StyleMarker(Style Style) : Markup
    {
        public static readonly StyleMarker Reset =
            new(
                new Style(
                    TextColor.Reset,
                    TextColor.Reset,
                    TextFormat.Reset
                )
            );

        public override string ToString()
            => Style.ToString();

        internal override string DbgStr()
            => "STY(" + Style.DbgStr() + ")";
    }

    [DebuggerStepThrough]
    public sealed record Text(string Content) : Markup
    {
        public override string ToString() => Content;

        internal override string DbgStr() => Content;
    }
}