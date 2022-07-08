using System.IO;

// !! WARNING !!
// I had to invoke dark powers to write the following functions, hopefully without bugs, so edit
// with *extreme* care.
// It takes way longer than it looks to figure out, so please again : beware !
public class SourceCode
{
    public string[] RawText { get; }

    public SourceCode(Uri path) {
        RawText = File.ReadAllLines(path.AbsolutePath);
    }

    public SourceCode(string text) {
        RawText = text.Split('\n');
    }

    public SourceCode(IEnumerable<string> lines) {
        RawText = lines.ToArray();
    }

    public string? GetRawLineAt(int lineNumber) {
        if (lineNumber < 1 || RawText.Length < lineNumber) return null;

        return RawText[lineNumber - 1];
    }

    public string GetPrettyLineAt(int lineNumber, string separator = "|") {
        var lineText = GetRawLineAt(lineNumber);

        if (lineText is null) return "";

        var padding = "\t";

        // this was carefully crafted in the depth of desperation, please please careful !
        // ...and yes, 100 is kinda arbitrary, but it's to be safe about range
        var n = Utilities.GetNumberOfDigits(lineNumber + 100) - Utilities.GetNumberOfDigits(lineNumber);

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
              Utilities.GetNumberOfDigits(lastLine + 100)   // the space the line number takes
            + 3                                             // the space for " > "
            + (firstColumn - 1)                             // the space before the character's column
        );

        output += "\t" + padding + "v error starts here\n";

        for (int i = 0; i < lineLength; i++) {
            var actualLine = GetPrettyLineAt(
                lineNumber: firstLine + i,
                separator: ">"
            );

            if (actualLine == "") {
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
              Utilities.GetNumberOfDigits(line + 100)   // the space the line number takes
            + 3                                         // the space for " | "
            + (column - 1)                              // the space before the character's column
        );

        var actualLine = GetPrettyLineAt(line);

        if (actualLine == "") {
            output += "EOF";

            return output;
        }

        output += actualLine + "\n\t"; // the tab is to adjust for the source code padding

        output += padding + new string('^', columnLength) + " right here\n";

        return output + FormatTextEpilogueAt(line);
    }

    public string FormatTextAtPoint(Location position) {
        var (line, column, _) = position;

        var output = FormatTextPreludeAt(line);

        var actualLine = GetPrettyLineAt(line);

        if (actualLine == "") {
            output += "EOF";

            return output;
        }

        output += actualLine + '\n';

        var padding = new string(' ',
              Utilities.GetNumberOfDigits(line + 100)   // the space the line number takes
            + 3                                         // the space for " | "
            + (position.column - 1)                     // the space before the character's column
        );

        output += '\t' + padding + "^ error starts here\n";

        return output + FormatTextEpilogueAt(line);
    }

}