[System.Diagnostics.DebuggerStepThrough]
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
}