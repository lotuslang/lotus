using System.Diagnostics;

[DebuggerStepThrough]
[DebuggerDisplay("{DbgString()}")]
internal record Style(
    TextColor? Foreground = null,
    TextColor? Background = null,
    TextFormat Format = TextFormat.None
)
{
    public override string ToString() {
        var output = "";

        if (Foreground is not null)
            output += Foreground.GetFGString();
        if (Background is not null)
            output += Background.GetBGString();

        return output + MarkupUtils.ToString(Format);
    }

    internal string DbgString() {
        var output = "";

        if (Foreground is not null) {
            output += Foreground.GetType().Name;
        }

        output += " / ";

        if (Background is not null) {
            output += Background.GetType().Name;
        }

        output += " / ";

        if (Format is not TextFormat.None) {
            output += Format.ToString() + "/";
        }

        return output[..^1];
    }
}