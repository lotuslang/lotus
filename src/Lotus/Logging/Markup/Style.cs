[DebuggerStepThrough]
[DebuggerDisplay("{DbgString()}")]
internal sealed record Style(
    TextColor? Foreground = null,
    TextColor? Background = null,
    TextFormat Format = TextFormat.None
)
{
    public override string ToString() {
        var output = "\x1b[";

        if (Foreground is not null) {
            output += ';' + Foreground.GetFGString();
        }

        if (Background is not null) {
            output += ';' + Background.GetBGString();
        }

        output += ';' + MarkupUtils.GetCode(Format);

        return output + ";10m";
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