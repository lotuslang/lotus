using System;

public record LocationRange(int firstLine, int lastLine, int firstColumn, int lastColumn, string filename = "<std>") : IComparable<LocationRange>
{
    public static readonly LocationRange NULL = new(Location.NULL, Location.NULL);

    public int LineLength => lastLine - firstLine + 1;

    public int ColumnLength => lastColumn - firstColumn + 1;

    public LocationRange(Location first, Location last) : this(first.line, last.line, first.column, last.column, first.filename) {
        if (first.filename != last.filename) {
            Logger.Warning(new InternalError() {
                Message = "Tried to created a LocationRange using locations that do not have the same origin/filename ("
                + System.IO.Path.GetFileName(first.filename)
                + " vs "
                + System.IO.Path.GetFileName(last.filename)
                + "). Setting filename to the first location's filename ('" + first.filename + "')",
                Location = first
            });
        }
    }

    public LocationRange(LocationRange first, LocationRange last) : this(first.firstLine, last.lastLine, first.firstColumn, last.lastColumn, first.filename) {
        if (first.filename != last.filename) {
            Logger.Warning(
                  "Tried to created a LocationRange using ranges that do not have the same origin/filename ("
                + System.IO.Path.GetFileName(first.filename)
                + " vs "
                + System.IO.Path.GetFileName(last.filename)
                + "). Setting filename to the first range's filename ('" + first.filename + "')",
                location: first
            );
        }
    }

    public void Deconstruct(out Location first, out Location last) {
        first = GetFirstLocation();

        last = GetLastLocation();
    }

    public void Deconstruct(
        out int firstLine,
        out int lastLine,
        out int lineLength,
        out int firstColumn,
        out int lastColumn,
        out int columnLength,
        out string filename
    ) {
        firstLine = this.firstLine;
        lastLine = this.lastLine;
        lineLength = LineLength;
        firstColumn = this.firstColumn;
        lastColumn = this.lastColumn;
        columnLength = ColumnLength;
        filename = this.filename;
    }

    public bool Contains(LocationRange range)
        => this != range && GetFirstLocation() > range.GetFirstLocation() && range.GetLastLocation() < GetLastLocation();

    public bool IsLaterThan(LocationRange range)
        => range.firstLine == this.firstLine
                ? range.firstColumn < this.firstColumn
                : range.firstLine < this.firstLine;

    public bool IsEarlierThan(LocationRange range)
        => range.firstLine == this.firstLine
                ? range.firstColumn > this.firstColumn
                : range.firstLine > this.firstLine;

    public bool IsSingleLocation() => LineLength == 1 && ColumnLength == 1;


    public Location GetFirstLocation() => new(firstLine, firstColumn, filename);

    public Location GetLastLocation() => new(lastLine, lastColumn, filename);


    public override string ToString() => this;

    public int CompareTo(LocationRange? other) {
        if (other is null) return -1;
        if (other == NULL) return this == NULL ? 0 : -1;
        if (this.filename != other.filename) return 0;

        // line
        var output = this.firstLine.CompareTo(other.firstLine);

        // column
        if (output == 0)
            output = this.firstColumn.CompareTo(other.firstColumn);
        else
            return output;

        // line length
        if (output == 0)
            output = this.LineLength.CompareTo(other.LineLength);
        else
            return output;

        // column length
        if (output == 0)
            output = this.ColumnLength.CompareTo(other.ColumnLength);

        return output;
    }

    public static implicit operator string(LocationRange range) {
        if (range.IsSingleLocation()) return range.GetFirstLocation();
        else return $"{range.filename}({range.firstLine}:{range.firstColumn} - {range.lastLine}:{range.lastColumn})";
    }
}