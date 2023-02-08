namespace Lotus.Text;

#pragma warning disable IDE1006
[DebuggerDisplay("{DbgStr(),nq}")]
public record struct LocationRange(int firstLine, int lastLine, int firstColumn, int lastColumn, string filename = "<std>") : IComparable<LocationRange>, ILocalized
{
    public static readonly LocationRange NULL = new(Location.NULL, Location.NULL);

    LocationRange ILocalized.Location => this;

    public int LineLength => lastLine - firstLine + 1;

    public int ColumnLength => lastColumn - firstColumn + 1;

    public LocationRange(Location first, Location last) : this(first.line, last.line, first.column, last.column, first.filename) {
        if (first.filename == "<std>")
            filename = last.filename;
    }

    public LocationRange(LocationRange first, LocationRange last) : this(first.firstLine, last.lastLine, first.firstColumn, last.lastColumn, first.filename) {
        if (first.filename == "<std>")
            filename = last.filename;
    }

    public LocationRange(ILocalized first, ILocalized last) : this(first.Location, last.Location) {}

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

    public bool IsAfter(LocationRange range)
        => range.firstLine == this.firstLine
                ? range.firstColumn < this.firstColumn
                : range.firstLine < this.firstLine;

    public bool IsBefore(LocationRange range)
        => range.firstLine == this.firstLine
                ? range.firstColumn > this.firstColumn
                : range.firstLine > this.firstLine;

    public bool IsSingleLocation() => LineLength == 1 && ColumnLength == 1;

    public Location GetFirstLocation() => new(firstLine, firstColumn, filename);

    public Location GetLastLocation() => new(lastLine, lastColumn, filename);

    public override string ToString()
        => IsSingleLocation()
                ? GetFirstLocation().ToString()
                : $"{filename}({firstLine}:{firstColumn} - {lastLine}:{lastColumn})";

    public int CompareTo(LocationRange other) {
        if (other == NULL) return this == NULL ? 0 : -1;
        if (this.filename != other.filename) return 0;

        if (firstLine != other.firstLine) {
            return firstLine < other.firstLine ? -1 : 1;
        }

        if (firstColumn != other.firstColumn) {
            return firstColumn < other.firstColumn ? -1 : 1;
        }

        if (LineLength != other.LineLength) {
            return LineLength < other.LineLength ? -1 : 1;
        }

        if (ColumnLength != other.ColumnLength) {
            return ColumnLength < other.ColumnLength ? -1 : 1;
        }

        return 0;
    }

    public static bool operator <(LocationRange loc1, LocationRange loc2)
        => loc1.CompareTo(loc2) < 0;
    public static bool operator <=(LocationRange loc1, LocationRange loc2)
        => loc1.CompareTo(loc2) <= 0;
    public static bool operator >(LocationRange loc1, LocationRange loc2)
        => loc1.CompareTo(loc2) > 0;
    public static bool operator >=(LocationRange loc1, LocationRange loc2)
        => loc1.CompareTo(loc2) >= 0;

    private string DbgStr()
        => $"{System.IO.Path.GetFileName(filename)}({firstLine}:{firstColumn} - {lastLine}:{lastColumn})";
}
