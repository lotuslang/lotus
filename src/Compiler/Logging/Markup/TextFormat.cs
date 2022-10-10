namespace Lotus.Text;

[Flags]
internal enum TextFormat {
    None = 0,
    Reset = 1,
    Bold = 2 * Reset,
    Faint = 2 * Bold,
    Underline = 2 * Faint,
    Italic = 2 * Underline,
    Strikethrough = 2 * Italic,
}