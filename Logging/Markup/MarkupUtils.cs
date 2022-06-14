internal static class MarkupUtils
{
    public static string GetFGString(this TextColor color) {
        var output = color.GetForegroundCode().ToString();

        if (color is TextColor.Custom custom) {
            output +=
                ";2;" + (custom.HexCode >> 16) // r
                      + ";" + ((custom.HexCode & 0x00ff00) >> 8) // g
                      + ";" + (custom.HexCode & 0x0000ff); // b
        }

        return output;
    }

    public static string GetBGString(this TextColor color) {
        var output = color.GetBackgroundCode().ToString();

        if (color is TextColor.Custom custom) {
            output +=
                ";2;" + (custom.HexCode >> 16) // r
                      + ";" + ((custom.HexCode & 0x00ff00) >> 8) // g
                      + ";" + (custom.HexCode & 0x0000ff); // b
        }

        return output;
    }

    public static string GetCode(this TextFormat? format) {
        var output = "";

        if (format is null or TextFormat.None)
            return output;

        if ((format & TextFormat.Reset) != 0) {
            //output += "\x1b[0m";
            output += //';' <-- useless if we're first
                  "22;" // bold + faint
                + "23;" // italic
                + "24;" // underline
                + "29;" // strikethrough
            ;
        }

        if ((format & TextFormat.Bold) != 0) {
            output += ";1";
        }

        if ((format & TextFormat.Faint) != 0) {
            output += ";2";
        }

        if ((format & TextFormat.Italic) != 0) {
            output += ";3";
        }

        if ((format & TextFormat.Underline) != 0) {
            output += ";4";
        }

        if ((format & TextFormat.Strikethrough) != 0) {
            output += ";9";
        }

        return output;

    }

    public static string ToString(this TextFormat? format)
        => format is null or TextFormat.None
                ? ""
                : "\x1b[" + GetCode(format) + 'm';
}