internal static class MarkupUtils
{
    public static string GetFGString(this TextColor color) {
        var output = "\x1b[" + color.GetForegroundCode();

        if (color is TextColor.Custom custom) {
            output +=
                ";2;" + (custom.HexCode >> 16) // r
                      + ";" + ((custom.HexCode & 0x00ff00) >> 8) // g
                      + ";" + (custom.HexCode & 0x0000ff); // b
        }

        return output + 'm';
    }

    public static string GetBGString(this TextColor color) {
        var output = "\x1b[" + color.GetBackgroundCode();

        if (color is TextColor.Custom custom) {
            output +=
                ";2;" + (custom.HexCode >> 16) // r
                      + ";" + ((custom.HexCode & 0x00ff00) >> 8) // g
                      + ";" + (custom.HexCode & 0x0000ff); // b
        }

        return output + 'm';
    }

    public static string ToString(this TextFormat format) {
        var output = "";

        if (format == TextFormat.None)
            return output;

        if ((format & TextFormat.Reset) != 0) {
            output += "\x1b[1m";
        }

        if ((format & TextFormat.Bold) != 0) {
            output += "\x1b[1m";
        }

        if ((format & TextFormat.Faint) != 0) {
            output += "\x1b[2m";
        }

        if ((format & TextFormat.Italic) != 0) {
            output += "\x1b[3m";
        }

        if ((format & TextFormat.Underline) != 0) {
            output += "\x1b[4m";
        }

        if ((format & TextFormat.Strikethrough) != 0) {
            output += "\x1b[9m";
        }

        return output;
    }
}