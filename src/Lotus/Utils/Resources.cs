internal static class Resources
{
    public static readonly HashSet<string> keywords = new() {
        "var",
        "new",
        "func",
        "return",
        "for",
        "while",
        "foreach",
        "in",
        "if",
        "else",
        "from",
        "continue",
        "break",

        "public",
        "internal",
        "protected",
        "private",

        "import",
        "using",
        "namespace",
        "enum",
        "struct",
    };

    public static readonly HashSet<string> internalFunctions = new() {
        "print",
    };
}