/*public struct LocationR
{
    public static readonly Location NULL = new Location(-1, -1);

    public int line;
    public int column;

    public readonly string filename;

    public Location(int line, int column, string filename = "<std>") {
        this.line = line;
        this.column = column;
        this.filename = filename;
    }

    public override string ToString()
        => this;

    public void Deconstruct(out int line, out int column, out string filename) {
        line = this.line;
        column = this.column;
        filename = this.filename;
    }

    public static implicit operator LocationRange(Location loc) => new LocationRange(loc, loc);

    public static implicit operator string(Location loc) {
        return $"{loc.filename}({loc.line}, {loc.column})";
    }
}*/

public record Location(int line, int column, string filename = "<std>") {
    public static readonly Location NULL = new Location(-1, -1);

    public static implicit operator LocationRange(Location loc) => new LocationRange(loc, loc);

    public static implicit operator string(Location loc) {
        return $"{loc.filename}({loc.line}, {loc.column})";
    }

    public static bool operator <(Location loc1, Location loc2)
        => loc1.line == loc2.line
                ? loc1.column < loc2.column
                : loc1.line < loc2.line;
    public static bool operator >(Location loc1, Location loc2)
        => loc1.line == loc2.line
                ? loc1.column > loc2.column
                : loc1.line > loc2.line;

    public override string ToString()
        => this;
}