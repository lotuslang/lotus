internal abstract record TextColor
{
    public abstract int GetForegroundCode();
    public abstract int GetBackgroundCode();

    public static Default DefaultColor = new Default();
    public record Default : TextColor
    {
        internal Default() {}

        public override int GetForegroundCode() => 0;
        public override int GetBackgroundCode() => 0;
    }

    public record Custom(int HexCode) : TextColor {
        public override int GetForegroundCode() => 32;
        public override int GetBackgroundCode() => 42;
    }

    public static Black BlackColor = new Black();
    public record Black : TextColor {

        internal Black() {}
        public override int GetForegroundCode() => 30;
        public override int GetBackgroundCode() => GetForegroundCode() + 10;
    }

    public static DarkBlue DarkBlueColor = new DarkBlue();
    public record DarkBlue : TextColor {

        internal DarkBlue() {}
        public override int GetForegroundCode() => 34;
        public override int GetBackgroundCode() => GetForegroundCode() + 10;
    }

    public static DarkGreen DarkGreenColor = new DarkGreen();
    public record DarkGreen : TextColor {

        internal DarkGreen() {}
        public override int GetForegroundCode() => 32;
        public override int GetBackgroundCode() => GetForegroundCode() + 10;
    }

    public static DarkCyan DarkCyanColor = new DarkCyan();
    public record DarkCyan : TextColor {

        internal DarkCyan() {}
        public override int GetForegroundCode() => 36;
        public override int GetBackgroundCode() => GetForegroundCode() + 10;
    }

    public static DarkRed DarkRedColor = new DarkRed();
    public record DarkRed : TextColor {

        internal DarkRed() {}
        public override int GetForegroundCode() => 31;
        public override int GetBackgroundCode() => GetForegroundCode() + 10;
    }

    public static DarkMagenta DarkMagentaColor = new DarkMagenta();
    public record DarkMagenta : TextColor {

        internal DarkMagenta() {}
        public override int GetForegroundCode() => 35;
        public override int GetBackgroundCode() => GetForegroundCode() + 10;
    }

    public static DarkYellow DarkYellowColor = new DarkYellow();
    public record DarkYellow : TextColor {

        internal DarkYellow() {}
        public override int GetForegroundCode() => 33;
        public override int GetBackgroundCode() => GetForegroundCode() + 10;
    }

    public static DarkGray DarkGrayColor = new DarkGray();
    public record DarkGray : TextColor {

        internal DarkGray() {}
        public override int GetForegroundCode() => 90;
        public override int GetBackgroundCode() => GetForegroundCode() + 10;
    }

    public static Gray GrayColor = new Gray();
    public record Gray : TextColor {

        internal Gray() {}
        public override int GetForegroundCode() => 37;
        public override int GetBackgroundCode() => GetForegroundCode() + 10;
    }

    public static Blue BlueColor = new Blue();
    public record Blue : TextColor {

        internal Blue() {}
        public override int GetForegroundCode() => 94;
        public override int GetBackgroundCode() => GetForegroundCode() + 10;
    }

    public static Green GreenColor = new Green();
    public record Green : TextColor {

        internal Green() {}
        public override int GetForegroundCode() => 92;
        public override int GetBackgroundCode() => GetForegroundCode() + 10;
    }

    public static Cyan CyanColor = new Cyan();
    public record Cyan : TextColor {

        internal Cyan() {}
        public override int GetForegroundCode() => 96;
        public override int GetBackgroundCode() => GetForegroundCode() + 10;
    }

    public static Red RedColor = new Red();
    public record Red : TextColor {

        internal Red() {}
        public override int GetForegroundCode() => 91;
        public override int GetBackgroundCode() => GetForegroundCode() + 10;
    }

    public static Magenta MagentaColor = new Magenta();
    public record Magenta : TextColor {

        internal Magenta() {}
        public override int GetForegroundCode() => 95;
        public override int GetBackgroundCode() => GetForegroundCode() + 10;
    }

    public static Yellow YellowColor = new Yellow();
    public record Yellow : TextColor {

        internal Yellow() {}
        public override int GetForegroundCode() => 93;
        public override int GetBackgroundCode() => GetForegroundCode() + 10;
    }

    public static White WhiteColor = new White();
    public record White : TextColor {

        internal White() {}
        public override int GetForegroundCode() => 97;
        public override int GetBackgroundCode() => GetForegroundCode() + 10;
    }
}