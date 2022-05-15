public record LocationRange(int firstLine, int lastLine, int firstColumn, int lastColumn, string filename)
{
    public static readonly LocationRange NULL = new(Location.NULL, Location.NULL);

    public int LineLength => lastLine - firstLine + 1;

    public int ColumnLength => lastColumn - firstColumn + 1;

    public LocationRange(Location first, Location last) : this(first.line, last.line, first.column, last.column, first.filename) {
        if (first.filename != last.filename) {
            Logger.Warning(new LotusException(
                message: "Tried to created a LocationRange using Locations that do not have the same origin/filename. "
                        + "Setting filename to the first Location's filename ('" + first.filename + "')",
                range: first
            ));
        }
    }

    public LocationRange(LocationRange first, LocationRange last) : this(first.firstLine, last.lastLine, first.firstColumn, last.lastColumn, first.filename) {
        if (first.filename != last.filename) {
            Logger.Warning(new LotusException(
                message: "Tried to created a LocationRange using LocationRanges that do not have the same origin/filename. "
                        + "Setting filename to the first Location's filename ('" + first.filename + "')",
                range: first
            ));
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

    public static LocationRange operator +(LocationRange left, LocationRange right)
        => new(left, right);

    public static implicit operator string(LocationRange range)
        => range.IsSingleLocation()
                ? range.GetFirstLocation()
                : $"{range.filename}({range.firstLine}:{range.firstColumn} ~ {range.lastLine}:{range.lastColumn})";
}