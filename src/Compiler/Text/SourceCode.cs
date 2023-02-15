namespace Lotus.Text;

// !! WARNING !!
// I had to invoke dark powers to write the following functions, hopefully without bugs, so edit
// with *extreme* care.
// It takes way longer than it looks to figure out, so please again: beware !
public sealed class SourceCode
{
    public ReadOnlyMemory<string> RawLines { get; }

    public SourceCode(string text) {
        RawLines = text.Split('\n');
    }

    public SourceCode(ReadOnlyMemory<string> lines) {
        RawLines = lines;
    }

    public SourceCode(string[] lines) {
        RawLines = lines.AsMemory();
    }

    public SourceCode(ImmutableArray<string> lines) {
        RawLines = lines.AsMemory();
    }

    public string? GetRawLineAtOrDefault(int lineNumber)
        => lineNumber < 1 || RawLines.Length < lineNumber
                ? null
                : RawLines.Span[lineNumber - 1];

    public string GetPrettyLineAt(int lineNumber, string separator = "|") {
        var lineText = GetRawLineAtOrDefault(lineNumber);

        if (lineText is null) return "";

        var padding = "\t";

        // this was carefully crafted in the depth of desperation, please please careful !
        // ...and yes, 100 is kinda arbitrary, but it's to be safe about range
        var n = MiscUtils.GetNumberOfDigits(lineNumber + 100) - MiscUtils.GetNumberOfDigits(lineNumber);

        if (n > 0) padding += new string(' ', n);

        return padding + lineNumber + " " + separator + " " + lineText;
    }

    public string FormatTextPreludeAt(int lineNumber) {
        var output = "";

        var pre2Line = GetPrettyLineAt(lineNumber - 2);

        if (pre2Line != "") output += pre2Line + '\n';

        var pre1Line = GetPrettyLineAt(lineNumber - 1);

        if (pre1Line != "") output += pre1Line + '\n';

        return output;
    }

    public string FormatTextEpilogueAt(int lineNumber) {
        var output = "";

        var post1Line = GetPrettyLineAt(lineNumber + 1);

        if (post1Line != "") output += post1Line + '\n';

        var post2Line = GetPrettyLineAt(lineNumber + 2);

        if (post2Line != "") output += post2Line + '\n';

        return output;
    }

    public string FormatTextAtLines(LocationRange range) {
        var (firstLine, lastLine, lineLength, firstColumn, _, _, _) = range;
        var output = FormatTextPreludeAt(firstLine);

        var padding = new string(' ',
              MiscUtils.GetNumberOfDigits(lastLine + 100)   // the space the line number takes
            + 3                                             // the space for " > "
            + (firstColumn - 1)                             // the space before the character's column
        );

        output += "\t" + padding + "v error starts here\n";

        for (int i = 0; i < lineLength; i++) {
            var actualLine = GetPrettyLineAt(
                lineNumber: firstLine + i,
                separator: ">"
            );

            if (actualLine?.Length == 0) {
                output += "EOF";

                return output;
            }

            output += actualLine + '\n';
        }

        return output + FormatTextEpilogueAt(lastLine);
    }

    public string FormatTextAtLine(LocationRange range) {
        var (line, _, _, column, _, columnLength, _) = range;

        var output = FormatTextPreludeAt(line);

        var padding = new string(' ',
              MiscUtils.GetNumberOfDigits(line + 100)   // the space the line number takes
            + 3                                         // the space for " | "
            + (column - 1)                              // the space before the character's column
        );

        var actualLine = GetPrettyLineAt(line);

        if (actualLine?.Length == 0) {
            output += "EOF";

            return output;
        }

        output += actualLine + "\n\t"; // the tab is to adjust for the source code padding

        output += padding + new string('^', columnLength) + " right here\n";

        return output + FormatTextEpilogueAt(line);
    }

    public string FormatTextAtPoint(Location position) {
        var (line, _, _) = position;

        var output = FormatTextPreludeAt(line);

        var actualLine = GetPrettyLineAt(line);

        if (actualLine?.Length == 0) {
            output += "EOF";

            return output;
        }

        output += actualLine + '\n';

        var padding = new string(' ',
              MiscUtils.GetNumberOfDigits(line + 100)   // the space the line number takes
            + 3                                         // the space for " | "
            + (position.column - 1)                     // the space before the character's column
        );

        output += '\t' + padding + "^ error starts here\n";

        return output + FormatTextEpilogueAt(line);
    }
}