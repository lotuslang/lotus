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

    public Location With(int line = Int32.MinValue, int column = Int32.MinValue, string? filename = null) {
        return new Location(
            line == -1 ? this.line : line,
            column == -1 ? this.column : column,
            filename ?? this.filename
        );
    }

    public override string ToString()
        => (string)this;

    public static implicit operator string(Location loc) {
        return $"{loc.filename}({loc.line}, {loc.column})";
    }
}