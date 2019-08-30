using System;

public struct Location
{
    public int line;
    public int column;

    public string filename;

    public Location(int l, int c, string fileName = "<std>") {
        line = l;
        column = c;
        filename = fileName;
    }

    public string Totext() {
        return $"In {filename}, at Line {line}, Column {column}";
    }

    public override string ToString()
        => (string)this;

    public static implicit operator string(Location loc) {
        return $"{loc.filename}({loc.line}, {loc.column})";
    }
}