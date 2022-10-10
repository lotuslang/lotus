namespace Lotus.Text;

#pragma warning disable IDE1006
[DebuggerDisplay("{DbgStr(),nq}")]
public sealed record Location(int line, int column, string filename = "<std>") : ILocalized
{
    public static readonly Location NULL = new(-1, -1);

    LocationRange ILocalized.Location => this;

    public override string ToString()
        => $"{filename}({line}:{column})";

    public static implicit operator LocationRange(Location loc)
        => new(loc, loc);

    public static bool operator <(Location loc1, Location loc2)
        => loc1.line == loc2.line
                ? loc1.column < loc2.column
                : loc1.line < loc2.line;
    public static bool operator >(Location loc1, Location loc2)
        => loc1.line == loc2.line
                ? loc1.column > loc2.column
                : loc1.line > loc2.line;

    private string DbgStr()
        => $"{System.IO.Path.GetFileName(filename)}({line}:{column})";
}