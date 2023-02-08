namespace Lotus.Text;

#pragma warning disable IDE1006
[DebuggerDisplay("{DbgStr(),nq}")]
public record struct Location(int line, int column, string filename = "<std>") : IComparable<Location>, ILocalized
{
    public static readonly Location NULL = new(-1, -1);

    LocationRange ILocalized.Location => this;

    public override string ToString()
        => $"{filename}({line}:{column})";

    public int CompareTo(Location other) {
        if (filename != other.filename)
            return 0;

        if (line != other.line)
            return line < other.line ? -1 : 1;

        if (column != other.column)
            return column < other.column ? -1 : 1;

        return 0;
    }

    public static implicit operator LocationRange(Location loc)
        => new(loc, loc);

    public static bool operator <(Location loc1, Location loc2)
        => loc1.CompareTo(loc2) < 0;
    public static bool operator <=(Location loc1, Location loc2)
        => loc1.CompareTo(loc2) <= 0;
    public static bool operator >(Location loc1, Location loc2)
        => loc1.CompareTo(loc2) > 0;
    public static bool operator >=(Location loc1, Location loc2)
        => loc1.CompareTo(loc2) >= 0;

    private string DbgStr()
        => $"{System.IO.Path.GetFileName(filename)}({line}:{column})";
}