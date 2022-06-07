public sealed class LotusGrammar : ReadOnlyGrammar
{
    private readonly Grammar internalGrammar = new();

    private static readonly LotusGrammar _instance = new();

    public static LotusGrammar Instance => _instance;

    private LotusGrammar() : base() {
        InitializeToklets();
        InitializeTriviaToklets();
        InitializeExpressionKinds();
        InitializeExpressionParslets();
        InitializeStatementParslets();
        InitializeTopLevelParslets();

        base.Initialize(internalGrammar);
    }

    private void InitializeTriviaToklets()
        => internalGrammar
            .RegisterToklet(NumberToklet.Instance)
            .RegisterToklet(ComplexStringToklet.Instance)
            .RegisterToklet(StringToklet.Instance)
            .RegisterToklet(IdentToklet.Instance)
            .RegisterToklet(OperatorToklet.Instance)
            .RegisterToklet(Toklet.Instance)
            ;

    private void InitializeToklets()
        => internalGrammar
            .RegisterTriviaToklet(CommentTriviaToklet.Instance)
            .RegisterTriviaToklet(WhitespaceTriviaToklet.Instance)
            .RegisterTriviaToklet(NewlineTriviaToklet.Instance)
            .RegisterTriviaToklet(new CharacterTriviaToklet(';'))
            .RegisterTriviaToklet(new TriviaToklet())
            ;

    private void InitializeExpressionParslets() {

        // values
        internalGrammar
            .RegisterPrefix(ExpressionKind.Number, NumberLiteralParslet.Instance)
            .RegisterPrefix(ExpressionKind.String, StringLiteralParslet.Instance)
            .RegisterPrefix(ExpressionKind.Identifier, IdentifierParslet.Instance)
            .RegisterPrefix(ExpressionKind.Boolean, BoolLiteralParslet.Instance)
            .RegisterPrefix(ExpressionKind.LeftParen, LeftParenParslet.Instance)
            ;

        // prefix operators
        internalGrammar
            .RegisterPrefixOperator(ExpressionKind.Plus, OperationType.Positive)
            .RegisterPrefixOperator(ExpressionKind.Minus, OperationType.Negative)
            .RegisterPrefixOperator(ExpressionKind.Not, OperationType.Not)
            .RegisterPrefixOperator(ExpressionKind.Increment, OperationType.PrefixIncrement)
            .RegisterPrefixOperator(ExpressionKind.Decrement, OperationType.PrefixDecrement)
            ;

        // misc prefix parslets
        internalGrammar
            .RegisterPrefix(ExpressionKind.Array, ArrayLiteralParslet.Instance)
            .RegisterPrefix(ExpressionKind.New, ObjectCreationParslet.Instance)
            ;

        // infix binary operators
        internalGrammar
            .RegisterInfixBinaryOperator(ExpressionKind.Plus, Precedence.Addition, OperationType.Addition)
            .RegisterInfixBinaryOperator(ExpressionKind.Minus, Precedence.Substraction, OperationType.Substraction)
            .RegisterInfixBinaryOperator(ExpressionKind.Multiply, Precedence.Multiplication, OperationType.Multiplication)
            .RegisterInfixBinaryOperator(ExpressionKind.Divide, Precedence.Division, OperationType.Division)
            .RegisterInfixBinaryOperator(ExpressionKind.Power, Precedence.Power, OperationType.Power)
            .RegisterInfixBinaryOperator(ExpressionKind.Modulo, Precedence.Modulo, OperationType.Modulo)
            .RegisterInfixBinaryOperator(ExpressionKind.Or, Precedence.Or, OperationType.Or)
            .RegisterInfixBinaryOperator(ExpressionKind.And, Precedence.And, OperationType.And)
            .RegisterInfixBinaryOperator(ExpressionKind.Xor, Precedence.Xor, OperationType.Xor)
            .RegisterInfixBinaryOperator(ExpressionKind.Eq, Precedence.Equal, OperationType.Equal)
            .RegisterInfixBinaryOperator(ExpressionKind.NotEq, Precedence.NotEqual, OperationType.NotEqual)
            .RegisterInfixBinaryOperator(ExpressionKind.Less, Precedence.LessThan, OperationType.Less)
            .RegisterInfixBinaryOperator(ExpressionKind.LessOrEq, Precedence.LessThanOrEqual, OperationType.LessOrEqual)
            .RegisterInfixBinaryOperator(ExpressionKind.Greater, Precedence.GreaterThan, OperationType.Greater)
            .RegisterInfixBinaryOperator(ExpressionKind.GreaterOrEq, Precedence.GreaterThanOrEqual, OperationType.GreaterOrEqual)
            .RegisterInfixBinaryOperator(ExpressionKind.Access, Precedence.Access, OperationType.Access)
            .RegisterInfixBinaryOperator(ExpressionKind.Assignment, Precedence.Assignment, OperationType.Assign)
            ;

        // misc infix parslets
        internalGrammar
            .RegisterInfix(ExpressionKind.Array, ArrayAccessParslet.Instance)
            .RegisterInfix(ExpressionKind.LeftParen, FuncCallParslet.Instance)
            .RegisterInfix(ExpressionKind.Ternary, TernaryOperatorParslet.Instance)
            ;

        // postfix operators
        internalGrammar
            .RegisterPostfixOperation(ExpressionKind.Increment, OperationType.PostfixIncrement)
            .RegisterPostfixOperation(ExpressionKind.Decrement, OperationType.PostfixDecrement)
            ;
    }

    private void InitializeExpressionKinds() {

        // Maths operators
        internalGrammar
            .RegisterExpressionKind("+", ExpressionKind.Plus)
            .RegisterExpressionKind("-", ExpressionKind.Minus)
            .RegisterExpressionKind("*", ExpressionKind.Multiply)
            .RegisterExpressionKind("/", ExpressionKind.Divide)
            .RegisterExpressionKind("%", ExpressionKind.Modulo)
            .RegisterExpressionKind("^", ExpressionKind.Power)
            ;

        // Logical/comparison operators
        internalGrammar
            .RegisterExpressionKind("!", ExpressionKind.Not)
            .RegisterExpressionKind("&&", ExpressionKind.And)
            .RegisterExpressionKind("||", ExpressionKind.Or)
            .RegisterExpressionKind("^^", ExpressionKind.Xor)
            .RegisterExpressionKind("<", ExpressionKind.Less)
            .RegisterExpressionKind("<=", ExpressionKind.LessOrEq)
            .RegisterExpressionKind(">", ExpressionKind.Greater)
            .RegisterExpressionKind(">=", ExpressionKind.GreaterOrEq)
            .RegisterExpressionKind("(", ExpressionKind.LeftParen)
            .RegisterExpressionKind("[", ExpressionKind.Array)
            .RegisterExpressionKind(".", ExpressionKind.Access)
            .RegisterExpressionKind("=", ExpressionKind.Assignment)
            .RegisterExpressionKind("==", ExpressionKind.Eq)
            .RegisterExpressionKind("!=", ExpressionKind.NotEq)
            ;

        // Misc
        internalGrammar
            .RegisterExpressionKind("++", ExpressionKind.Increment)
            .RegisterExpressionKind("--", ExpressionKind.Decrement)
            .RegisterExpressionKind("new", ExpressionKind.New)
            .RegisterExpressionKind("?", ExpressionKind.Ternary)
            ;
    }

    private void InitializeStatementParslets()
        => internalGrammar
            .RegisterStatementParslet("var", DeclarationParslet.Instance)
            .RegisterStatementParslet("func", FunctionDeclarationParslet.Instance)
            .RegisterStatementParslet("return", ReturnParslet.Instance)
            .RegisterStatementParslet("foreach", ForeachParslet.Instance)
            .RegisterStatementParslet("for", ForParslet.Instance)
            .RegisterStatementParslet("if", IfParslet.Instance)
            .RegisterStatementParslet("while", WhileParslet.Instance)
            .RegisterStatementParslet("do", DoWhileParslet.Instance)
            .RegisterStatementParslet("break", BreakParslet.Instance)
            .RegisterStatementParslet("continue", ContinueParslet.Instance)
            .RegisterStatementParslet("print", PrintParslet.Instance)
            ;

    private void InitializeTopLevelParslets()
        => internalGrammar
            .RegisterTopLevelParslets("namespace", NamespaceParslet.Instance)
            .RegisterTopLevelParslets("from", ImportParslet.Instance)
            .RegisterTopLevelParslets("using", UsingParslet.Instance)
            ;
}