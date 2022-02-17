//[System.Flags]
public enum ErrorArea {
    Tokenizer = 1,
    Parser = 2,
    ASTHelper = 4,
    TypeChecker = 8,
}

internal static class ErrorAreaExtension {
    public static string ToName(this ErrorArea area)
        => area switch {
            ErrorArea.TypeChecker => "Type checking",
            ErrorArea.ASTHelper   => "AST Utilities",
            _ => System.Enum.GetName(area) ?? "unknown"
        };
}