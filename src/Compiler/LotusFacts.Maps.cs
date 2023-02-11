using Lotus.Syntax;

namespace Lotus;

public static partial class LotusFacts
{
    private static readonly Dictionary<string, ExpressionKind> _strToExprKinds = new() {
        { "+", ExpressionKind.Plus },
        { "-", ExpressionKind.Minus },
        { "*", ExpressionKind.Multiply },
        { "/", ExpressionKind.Divide },
        { "%", ExpressionKind.Modulo },
        { "^", ExpressionKind.Power },

        //logical operators
        { "!",  ExpressionKind.Not },
        { "&&", ExpressionKind.And },
        { "||", ExpressionKind.Or },
        { "^^", ExpressionKind.Xor },
        { "<",  ExpressionKind.Less },
        { "<=", ExpressionKind.LessOrEq },
        { ">",  ExpressionKind.Greater },
        { ">=", ExpressionKind.GreaterOrEq },
        { "(",  ExpressionKind.LeftParen },
        { "[",  ExpressionKind.Array },
        { ".",  ExpressionKind.Access },
        { "=",  ExpressionKind.Assignment },
        { "==", ExpressionKind.Eq },
        { "!=", ExpressionKind.NotEq },

        //misc
        { "++",  ExpressionKind.Increment },
        { "--",  ExpressionKind.Decrement },
        { "new", ExpressionKind.New },
        { "?",   ExpressionKind.Ternary },
    };

    private static readonly Dictionary<ExpressionKind, Precedence> _exprToPrecedence = new() {
        { ExpressionKind.Array,         ArrayAccessParslet.Instance.Precedence },
        { ExpressionKind.LeftParen,     FuncCallParslet.Instance.Precedence },
        { ExpressionKind.Ternary,       TernaryOperatorParslet.Instance.Precedence },
        { ExpressionKind.Access,        DotAccessParslet.Instance.Precedence },

        { ExpressionKind.Plus,          Precedence.Addition },
        { ExpressionKind.Minus,         Precedence.Subtraction },
        { ExpressionKind.Multiply,      Precedence.Multiplication },
        { ExpressionKind.Divide,        Precedence.Division },
        { ExpressionKind.Power,         Precedence.Power },
        { ExpressionKind.Modulo,        Precedence.Modulo },
        { ExpressionKind.Or,            Precedence.Or },
        { ExpressionKind.And,           Precedence.And },
        { ExpressionKind.Xor,           Precedence.Xor },
        { ExpressionKind.Eq,            Precedence.Equal },
        { ExpressionKind.NotEq,         Precedence.NotEqual },
        { ExpressionKind.Less,          Precedence.LessThan },
        { ExpressionKind.LessOrEq,      Precedence.LessThanOrEqual },
        { ExpressionKind.Greater,       Precedence.GreaterThan },
        { ExpressionKind.GreaterOrEq,   Precedence.GreaterThanOrEqual },
        { ExpressionKind.Assignment,    Precedence.Assignment },
    };

    private static readonly Dictionary<ExpressionKind, IInfixParslet<ValueNode>> _exprToInfixParslets = new() {
        { ExpressionKind.Array,         ArrayAccessParslet.Instance },
        { ExpressionKind.LeftParen,     FuncCallParslet.Instance },
        { ExpressionKind.Ternary,       TernaryOperatorParslet.Instance },
        { ExpressionKind.Access,        DotAccessParslet.Instance },

        { ExpressionKind.Plus,          new BinaryOperatorParslet(Precedence.Addition, OperationType.Addition) },
        { ExpressionKind.Minus,         new BinaryOperatorParslet(Precedence.Subtraction, OperationType.Subtraction) },
        { ExpressionKind.Multiply,      new BinaryOperatorParslet(Precedence.Multiplication, OperationType.Multiplication) },
        { ExpressionKind.Divide,        new BinaryOperatorParslet(Precedence.Division, OperationType.Division) },
        { ExpressionKind.Power,         new BinaryOperatorParslet(Precedence.Power, OperationType.Power) },
        { ExpressionKind.Modulo,        new BinaryOperatorParslet(Precedence.Modulo, OperationType.Modulo) },
        { ExpressionKind.Or,            new BinaryOperatorParslet(Precedence.Or, OperationType.Or) },
        { ExpressionKind.And,           new BinaryOperatorParslet(Precedence.And, OperationType.And) },
        { ExpressionKind.Xor,           new BinaryOperatorParslet(Precedence.Xor, OperationType.Xor) },
        { ExpressionKind.Eq,            new BinaryOperatorParslet(Precedence.Equal, OperationType.Equal) },
        { ExpressionKind.NotEq,         new BinaryOperatorParslet(Precedence.NotEqual, OperationType.NotEqual) },
        { ExpressionKind.Less,          new BinaryOperatorParslet(Precedence.LessThan, OperationType.Less) },
        { ExpressionKind.LessOrEq,      new BinaryOperatorParslet(Precedence.LessThanOrEqual, OperationType.LessOrEqual) },
        { ExpressionKind.Greater,       new BinaryOperatorParslet(Precedence.GreaterThan, OperationType.Greater) },
        { ExpressionKind.GreaterOrEq,   new BinaryOperatorParslet(Precedence.GreaterThanOrEqual, OperationType.GreaterOrEqual) },
        { ExpressionKind.Assignment,    new BinaryOperatorParslet(Precedence.Assignment, OperationType.Assign) },
    };

    private static readonly Dictionary<ExpressionKind, IPrefixParslet<ValueNode>> _exprToPrefixParslets = new() {
        // values
        { ExpressionKind.Number,        NumberLiteralParslet.Instance },
        { ExpressionKind.String,        StringLiteralParslet.Instance },
        { ExpressionKind.Identifier,    IdentifierParslet.Instance },
        { ExpressionKind.Boolean,       BoolLiteralParslet.Instance },
        { ExpressionKind.LeftParen,     LeftParenParslet.Instance },

        // normal prefix operators
        { ExpressionKind.Plus,          new PrefixOperatorParslet(OperationType.Positive) },
        { ExpressionKind.Minus,         new PrefixOperatorParslet(OperationType.Negative) },
        { ExpressionKind.Not,           new PrefixOperatorParslet(OperationType.Not) },
        { ExpressionKind.Increment,     new PrefixOperatorParslet(OperationType.PrefixIncrement) },
        { ExpressionKind.Decrement,     new PrefixOperatorParslet(OperationType.PrefixDecrement) },

        // misc prefix parslets
        { ExpressionKind.Array, ArrayLiteralParslet.Instance },
        { ExpressionKind.New, ObjectCreationParslet.Instance },
    };

    private static readonly Dictionary<ExpressionKind, IPostfixParslet<ValueNode>> _exprToPostfixParslets = new() {
        { ExpressionKind.Increment, new PostfixOperatorParslet(OperationType.PostfixIncrement) },
        { ExpressionKind.Decrement, new PostfixOperatorParslet(OperationType.PostfixDecrement) },
    };

    private static readonly Dictionary<string, IStatementParslet<StatementNode>> _strToStmtParslets = new() {
        { "var", DeclarationParslet.Instance },
        { "func", FunctionDeclarationParslet.Instance },
        { "return", ReturnParslet.Instance },
        { "foreach", ForeachParslet.Instance },
        { "for", ForParslet.Instance },
        { "if", IfParslet.Instance },
        { "while", WhileParslet.Instance },
        { "do", DoWhileParslet.Instance },
        { "break", BreakParslet.Instance },
        { "continue", ContinueParslet.Instance },
        { "print", PrintParslet.Instance },
    };

    private static readonly Dictionary<string, ITopLevelParslet<TopLevelNode>> _strToTopLevelParslets = new() {
        { "namespace", NamespaceParslet.Instance },
        { "import", ImportParslet.Instance },
        { "using", UsingParslet.Instance },
        { "enum", EnumParslet.Instance },
        { "struct", StructParslet.Instance },
    };

    private static readonly HashSet<string> _modifiers = new() {
        "private",
        "protected",
        "internal",
        "public"
    };

    private static readonly string[] _additionalKeywords = new[] {
        "from",
        "new"
    };

    private static readonly HashSet<string> _keywords;
    static LotusFacts() {
#pragma warning disable IDE0058
        _keywords = new HashSet<string>();

        foreach (var (keyword, _) in _strToStmtParslets)
            _keywords.Add(keyword);
        foreach (var (keyword, _) in _strToTopLevelParslets)
            _keywords.Add(keyword);
        foreach (var keyword in _modifiers)
            _keywords.Add(keyword);
        foreach (var keyword in _additionalKeywords)
            _keywords.Add(keyword);

#pragma warning restore IDE0058
    }
}