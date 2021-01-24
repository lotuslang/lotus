public readonly struct LocationRange
{
    public static readonly LocationRange NULL = new LocationRange(Location.NULL, Location.NULL);

    public readonly int firstLine;

    public readonly int lastLine;

    public readonly int LineLength => lastLine - firstLine + 1;

    public readonly int firstColumn;

    public readonly int lastColumn;

    public readonly int ColumnLength => lastColumn - firstColumn + 1;

    public readonly string filename;

    public LocationRange(int firstLine, int lastLine, int firstColumn, int lastColumn, string filename = "<std>") {
        this.firstLine = firstLine;
        this.lastLine = lastLine;

        this.firstColumn = firstColumn;
        this.lastColumn = lastColumn;

        this.filename = filename;
    }

    public LocationRange(Location first, Location last) : this(first.line, last.line, first.column, last.column, first.filename) {
        if (first.filename != last.filename) {
            Logger.Error(new LotusException(
                message: "Tried to created a LocationRange using Locations that do not have the same origin/filename. "
                        + "Setting filename to the first Location's filename ('" + first.filename + "')",
                range: first
            ));
        }
    }

    public LocationRange(LocationRange first, LocationRange last) : this(first.firstLine, last.lastLine, first.firstColumn, last.lastColumn, first.filename) {
        if (first.filename != last.filename) {
            Logger.Warning(
                  "Tried to created a LocationRange using LocationRanges that do not have the same origin/filename. "
                + "Setting filename to the first Location's filename ('" + first.filename + "')",
                location: first
            );
        }
    }

    public void Deconstruct(out Location first, out Location last) {
        first = GetFirstLocation();

        last = GetLastLocation();
    }

    public void Deconstruct(out int firstLine, out int lastLine, out int lineLength, out int firstColumn, out int lastColumn, out int columnLength, out string filename) {
        firstLine = this.firstLine;
        lastLine = this.lastLine;
        lineLength = LineLength;
        firstColumn = this.firstColumn;
        lastColumn = this.lastColumn;
        columnLength = ColumnLength;
        filename = this.filename;
    }

    public Location GetFirstLocation() => new Location(firstLine, firstColumn, filename);

    public Location GetLastLocation() => new Location(lastLine, lastColumn, filename);

    public bool IsSingleLocation() => LineLength == 1 && ColumnLength == 1;

    public override string ToString() => this;

    public static implicit operator string(LocationRange range) {
        if (range.IsSingleLocation()) return range.GetFirstLocation();
        else return $"{range.filename}({range.firstLine}, {range.firstColumn} : {range.lastLine}, {range.lastColumn})";
    }
}