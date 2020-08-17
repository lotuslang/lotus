using System;

public struct Location
{
    public int line;
    public int column;

    public string filename;

    public Location(int line, int column, string filename = "<std>") {
        this.line = line;
        this.column = column;
        this.filename = filename;
    }

    public string Totext() {
        return $"In {filename}, at Line {line}, Column {column}";
    }

    public override string ToString()
        => (string)this;

    public void Deconstruct(out int line, out int column, out string filename) {
        line = this.line;
        column = this.column;
        filename = this.filename;
    }

    public static implicit operator string(Location loc) {
        return $"{loc.filename}({loc.line}, {loc.column})";
    }
}