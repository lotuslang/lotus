namespace Lotus.Text;

internal abstract record TextColor
{
    public abstract int GetForegroundCode();
    public abstract int GetBackgroundCode();

    public static readonly TextColor Reset = new ResetColor();
    private sealed record ResetColor : TextColor {
        internal ResetColor() {}

        public override int GetForegroundCode() => 39;
        public override int GetBackgroundCode() => 49;
    }

    public static TextColor From(byte red, byte green, byte blue)
        => From(red + ((int)green << 8) + ((int)blue << 16));
    public static TextColor From(int hexCode)
        => new Custom(hexCode);
    internal sealed record Custom(int HexCode) : TextColor {
        public override int GetForegroundCode() => 32;
        public override int GetBackgroundCode() => 42;
    }

    public static readonly TextColor Black = new BlackColor();
    private sealed record BlackColor : TextColor {
        internal BlackColor() {}
        public override int GetForegroundCode() => 30;
        public override int GetBackgroundCode() => GetForegroundCode() + 10;
    }

    public static readonly TextColor DarkBlue = new DarkBlueColor();
    private sealed record DarkBlueColor : TextColor {
        internal DarkBlueColor() {}
        public override int GetForegroundCode() => 34;
        public override int GetBackgroundCode() => GetForegroundCode() + 10;
    }

    public static readonly TextColor DarkGreen = new DarkGreenColor();
    private sealed record DarkGreenColor : TextColor {
        internal DarkGreenColor() {}
        public override int GetForegroundCode() => 32;
        public override int GetBackgroundCode() => GetForegroundCode() + 10;
    }

    public static readonly TextColor DarkCyan = new DarkCyanColor();
    private sealed record DarkCyanColor : TextColor {
        internal DarkCyanColor() {}
        public override int GetForegroundCode() => 36;
        public override int GetBackgroundCode() => GetForegroundCode() + 10;
    }

    public static readonly TextColor DarkRed = new DarkRedColor();
    private sealed record DarkRedColor : TextColor {
        internal DarkRedColor() {}
        public override int GetForegroundCode() => 31;
        public override int GetBackgroundCode() => GetForegroundCode() + 10;
    }

    public static readonly TextColor DarkMagenta = new DarkMagentaColor();
    private sealed record DarkMagentaColor : TextColor {
        internal DarkMagentaColor() {}
        public override int GetForegroundCode() => 35;
        public override int GetBackgroundCode() => GetForegroundCode() + 10;
    }

    public static readonly TextColor DarkYellow = new DarkYellowColor();
    private sealed record DarkYellowColor : TextColor {
        internal DarkYellowColor() {}
        public override int GetForegroundCode() => 33;
        public override int GetBackgroundCode() => GetForegroundCode() + 10;
    }

    public static readonly TextColor DarkGray = new DarkGrayColor();
    private sealed record DarkGrayColor : TextColor {
        internal DarkGrayColor() {}
        public override int GetForegroundCode() => 90;
        public override int GetBackgroundCode() => GetForegroundCode() + 10;
    }

    public static readonly TextColor Gray = new GrayColor();
    private sealed record GrayColor : TextColor {
        internal GrayColor() {}
        public override int GetForegroundCode() => 37;
        public override int GetBackgroundCode() => GetForegroundCode() + 10;
    }

    public static readonly TextColor Blue = new BlueColor();
    private sealed record BlueColor : TextColor {
        internal BlueColor() {}
        public override int GetForegroundCode() => 94;
        public override int GetBackgroundCode() => GetForegroundCode() + 10;
    }

    public static readonly TextColor Green = new GreenColor();
    private sealed record GreenColor : TextColor {
        internal GreenColor() {}
        public override int GetForegroundCode() => 92;
        public override int GetBackgroundCode() => GetForegroundCode() + 10;
    }

    public static readonly TextColor Cyan = new CyanColor();
    private sealed record CyanColor : TextColor {
        internal CyanColor() {}
        public override int GetForegroundCode() => 96;
        public override int GetBackgroundCode() => GetForegroundCode() + 10;
    }

    public static readonly TextColor Red = new RedColor();
    private sealed record RedColor : TextColor {
        internal RedColor() {}
        public override int GetForegroundCode() => 91;
        public override int GetBackgroundCode() => GetForegroundCode() + 10;
    }

    public static readonly TextColor Magenta = new MagentaColor();
    private sealed record MagentaColor : TextColor {
        internal MagentaColor() {}
        public override int GetForegroundCode() => 95;
        public override int GetBackgroundCode() => GetForegroundCode() + 10;
    }

    public static readonly TextColor Yellow = new YellowColor();
    private sealed record YellowColor : TextColor {
        internal YellowColor() {}
        public override int GetForegroundCode() => 93;
        public override int GetBackgroundCode() => GetForegroundCode() + 10;
    }

    public static readonly TextColor White = new WhiteColor();
    private sealed record WhiteColor : TextColor {
        internal WhiteColor() {}
        public override int GetForegroundCode() => 97;
        public override int GetBackgroundCode() => GetForegroundCode() + 10;
    }
}