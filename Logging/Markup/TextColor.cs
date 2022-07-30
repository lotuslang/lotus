internal abstract record TextColor
{
    public abstract int GetForegroundCode();
    public abstract int GetBackgroundCode();

    public static readonly Reset ResetColor = new Reset();
    public sealed record Reset : TextColor {

        internal Reset() {}

        public override int GetForegroundCode() => 39;
        public override int GetBackgroundCode() => 49;
    }

    public sealed record Custom(int HexCode) : TextColor {
        public override int GetForegroundCode() => 32;
        public override int GetBackgroundCode() => 42;
    }

    public static readonly Black BlackColor = new Black();
    public sealed record Black : TextColor {

        internal Black() {}
        public override int GetForegroundCode() => 30;
        public override int GetBackgroundCode() => GetForegroundCode() + 10;
    }

    public static readonly DarkBlue DarkBlueColor = new DarkBlue();
    public sealed record DarkBlue : TextColor {

        internal DarkBlue() {}
        public override int GetForegroundCode() => 34;
        public override int GetBackgroundCode() => GetForegroundCode() + 10;
    }

    public static readonly DarkGreen DarkGreenColor = new DarkGreen();
    public sealed record DarkGreen : TextColor {

        internal DarkGreen() {}
        public override int GetForegroundCode() => 32;
        public override int GetBackgroundCode() => GetForegroundCode() + 10;
    }

    public static readonly DarkCyan DarkCyanColor = new DarkCyan();
    public sealed record DarkCyan : TextColor {

        internal DarkCyan() {}
        public override int GetForegroundCode() => 36;
        public override int GetBackgroundCode() => GetForegroundCode() + 10;
    }

    public static readonly DarkRed DarkRedColor = new DarkRed();
    public sealed record DarkRed : TextColor {

        internal DarkRed() {}
        public override int GetForegroundCode() => 31;
        public override int GetBackgroundCode() => GetForegroundCode() + 10;
    }

    public static readonly DarkMagenta DarkMagentaColor = new DarkMagenta();
    public sealed record DarkMagenta : TextColor {

        internal DarkMagenta() {}
        public override int GetForegroundCode() => 35;
        public override int GetBackgroundCode() => GetForegroundCode() + 10;
    }

    public static readonly DarkYellow DarkYellowColor = new DarkYellow();
    public sealed record DarkYellow : TextColor {

        internal DarkYellow() {}
        public override int GetForegroundCode() => 33;
        public override int GetBackgroundCode() => GetForegroundCode() + 10;
    }

    public static readonly DarkGray DarkGrayColor = new DarkGray();
    public sealed record DarkGray : TextColor {

        internal DarkGray() {}
        public override int GetForegroundCode() => 90;
        public override int GetBackgroundCode() => GetForegroundCode() + 10;
    }

    public static readonly Gray GrayColor = new Gray();
    public sealed record Gray : TextColor {

        internal Gray() {}
        public override int GetForegroundCode() => 37;
        public override int GetBackgroundCode() => GetForegroundCode() + 10;
    }

    public static readonly Blue BlueColor = new Blue();
    public sealed record Blue : TextColor {

        internal Blue() {}
        public override int GetForegroundCode() => 94;
        public override int GetBackgroundCode() => GetForegroundCode() + 10;
    }

    public static readonly Green GreenColor = new Green();
    public sealed record Green : TextColor {

        internal Green() {}
        public override int GetForegroundCode() => 92;
        public override int GetBackgroundCode() => GetForegroundCode() + 10;
    }

    public static readonly Cyan CyanColor = new Cyan();
    public sealed record Cyan : TextColor {

        internal Cyan() {}
        public override int GetForegroundCode() => 96;
        public override int GetBackgroundCode() => GetForegroundCode() + 10;
    }

    public static readonly Red RedColor = new Red();
    public sealed record Red : TextColor {

        internal Red() {}
        public override int GetForegroundCode() => 91;
        public override int GetBackgroundCode() => GetForegroundCode() + 10;
    }

    public static readonly Magenta MagentaColor = new Magenta();
    public sealed record Magenta : TextColor {

        internal Magenta() {}
        public override int GetForegroundCode() => 95;
        public override int GetBackgroundCode() => GetForegroundCode() + 10;
    }

    public static readonly Yellow YellowColor = new Yellow();
    public sealed record Yellow : TextColor {

        internal Yellow() {}
        public override int GetForegroundCode() => 93;
        public override int GetBackgroundCode() => GetForegroundCode() + 10;
    }

    public static readonly White WhiteColor = new White();
    public sealed record White : TextColor {

        internal White() {}
        public override int GetForegroundCode() => 97;
        public override int GetBackgroundCode() => GetForegroundCode() + 10;
    }
}