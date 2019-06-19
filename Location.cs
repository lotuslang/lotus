using System;

// TODO :
// - Add a `file` field


public struct Location
{
    public int line;
    public int column;

    public Location(int l, int c) {
        line = l;
        column = c;
    }

    public string Totext() {
        return $"Line {line}, Column {column}";
    }

    public override string ToString() => (string)this;

    public static implicit operator string(Location loc) {
        return $"({loc.line}, {loc.column})";
    }
}