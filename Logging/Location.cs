public record Location(int line, int column, string filename = "<std>")
{
    public static readonly Location NULL = new(-1, -1);

    public override string ToString()
        => this;

    public void Deconstruct(out int line, out int column, out string filename) {
        line = this.line;
        column = this.column;
        filename = this.filename;
    }

    public static implicit operator LocationRange(Location loc)
        => new(loc, loc);

    public static implicit operator string(Location loc)
        => $"{loc.filename}({loc.line}:{loc.column})";

    public static bool operator <(Location loc1, Location loc2)
        => loc1.line == loc2.line
                ? loc1.column < loc2.column
                : loc1.line < loc2.line;
    public static bool operator >(Location loc1, Location loc2)
        => loc1.line == loc2.line
                ? loc1.column > loc2.column
                : loc1.line > loc2.line;
}