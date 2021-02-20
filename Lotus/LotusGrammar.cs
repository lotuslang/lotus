public sealed class LotusGrammar : ReadOnlyGrammar
{
    private readonly Grammar internalGrammar = new Grammar();

    private static readonly LotusGrammar _instance = new LotusGrammar();

    public static LotusGrammar Instance => _instance;

    private LotusGrammar() : base() {
        InitializeToklets();
        InitializeTriviaToklets();
        InitializeExpressionKinds();
        InitializeExpressionParselets();
        InitializeStatementKinds();
        InitializeStatementParselets();

        base.Initialize(internalGrammar);
    }

    private void InitializeTriviaToklets() {
        internalGrammar
            .RegisterToklet(new NumberToklet())
            .RegisterToklet(new ComplexStringToklet())
            .RegisterToklet(new StringToklet())
            .RegisterToklet(new IdentToklet())
            .RegisterToklet(new OperatorToklet())
            .RegisterToklet(new Toklet());
    }

    private void InitializeToklets() {
        internalGrammar
            .RegisterTriviaToklet(new CommentTriviaToklet())
            .RegisterTriviaToklet(new WhitespaceTriviaToklet())
            .RegisterTriviaToklet(new NewlineTriviaToklet())
            .RegisterTriviaToklet(new CharacterTriviaToklet(';'))
            .RegisterTriviaToklet(new TriviaToklet());
    }

    private void InitializeExpressionParselets() {

        // values
        internalGrammar
            .RegisterPrefix(ExpressionKind.Number, new NumberLiteralParselet())
            .RegisterPrefix(ExpressionKind.String, new StringLiteralParselet())
            .RegisterPrefix(ExpressionKind.Identifier, new IdentifierParselet())
            .RegisterPrefix(ExpressionKind.Boolean, new BoolLiteralParselet())
            .RegisterPrefix(ExpressionKind.LeftParen, new LeftParenParselet());

        // prefix operators
        internalGrammar
            .RegisterPrefixOperator(ExpressionKind.Plus, OperationType.Positive)
            .RegisterPrefixOperator(ExpressionKind.Minus, OperationType.Negative)
            .RegisterPrefixOperator(ExpressionKind.Not, OperationType.Not)
            .RegisterPrefixOperator(ExpressionKind.Increment, OperationType.PrefixIncrement)
            .RegisterPrefixOperator(ExpressionKind.Decrement, OperationType.PrefixDecrement);

        // misc prefix parselets
        internalGrammar
            .RegisterPrefix(ExpressionKind.Array, new ArrayLiteralParselet())
            .RegisterPrefix(ExpressionKind.New, new ObjectCreationParselet());

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
            .RegisterInfixBinaryOperator(ExpressionKind.Assignment, Precedence.Assignment, OperationType.Assign);

        // misc infix parselets
        internalGrammar
            .RegisterInfix(ExpressionKind.Array, new ArrayAccessParselet())
            .RegisterInfix(ExpressionKind.LeftParen, new FuncCallParselet())
            .RegisterInfix(ExpressionKind.Ternary, new TernaryOperatorParselet());

        // postfix operators
        internalGrammar
            .RegisterPostfixOperation(ExpressionKind.Increment, OperationType.PostfixIncrement)
            .RegisterPostfixOperation(ExpressionKind.Decrement, OperationType.PostfixDecrement);
    }

    private void InitializeExpressionKinds() {

        // Maths operators
        internalGrammar
            .RegisterExpressionKind("+", ExpressionKind.Plus)
            .RegisterExpressionKind("-", ExpressionKind.Minus)
            .RegisterExpressionKind("*", ExpressionKind.Multiply)
            .RegisterExpressionKind("/", ExpressionKind.Divide)
            .RegisterExpressionKind("%", ExpressionKind.Modulo)
            .RegisterExpressionKind("^", ExpressionKind.Power);

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
            .RegisterExpressionKind("!=", ExpressionKind.NotEq);

        // Misc
        internalGrammar
            .RegisterExpressionKind("++", ExpressionKind.Increment)
            .RegisterExpressionKind("--", ExpressionKind.Decrement)
            .RegisterExpressionKind("new", ExpressionKind.New)
            .RegisterExpressionKind("?", ExpressionKind.Ternary);
    }

    private void InitializeStatementKinds() {
        internalGrammar
            .RegisterStatementKind("var", StatementKind.VariableDeclaration)
            .RegisterStatementKind("func", StatementKind.FunctionDeclaration)
            .RegisterStatementKind("return", StatementKind.ReturnStatement)
            .RegisterStatementKind("from", StatementKind.FromStatement)
            //TODO:.RegisterStatementKind("namespace", StatementKind.NamespaceStatement)
            .RegisterStatementKind("foreach", StatementKind.ForeachLoop)
            .RegisterStatementKind("for", StatementKind.ForLoop)
            .RegisterStatementKind("if", StatementKind.IfStatement)
            .RegisterStatementKind("while", StatementKind.WhileStatement)
            .RegisterStatementKind("do", StatementKind.DoWhileStatement)
            .RegisterStatementKind("break", StatementKind.BreakStatement)
            .RegisterStatementKind("continue", StatementKind.ContinueStatement)
            .RegisterStatementKind("using", StatementKind.UsingStatement)
            .RegisterStatementKind("print", StatementKind.PrintStatement);
    }

    private void InitializeStatementParselets() {
        internalGrammar
            .RegisterStatementParselet(StatementKind.VariableDeclaration, new DeclarationParselet())
            .RegisterStatementParselet(StatementKind.FunctionDeclaration, new FunctionDeclarationParselet())
            .RegisterStatementParselet(StatementKind.ReturnStatement, new ReturnParselet())
            .RegisterStatementParselet(StatementKind.FromStatement, new ImportParselet())
            //TODO:.RegisterStatementParselet(StatementKind.NamespaceStatement, new NamespaceParselet())
            .RegisterStatementParselet(StatementKind.ForeachLoop, new ForeachParselet())
            .RegisterStatementParselet(StatementKind.ForLoop, new ForParselet())
            .RegisterStatementParselet(StatementKind.IfStatement, new IfParselet())
            .RegisterStatementParselet(StatementKind.WhileStatement, new WhileParselet())
            .RegisterStatementParselet(StatementKind.DoWhileStatement, new DoWhileParselet())
            .RegisterStatementParselet(StatementKind.BreakStatement, new BreakParselet())
            .RegisterStatementParselet(StatementKind.ContinueStatement, new ContinueParselet())
            .RegisterStatementParselet(StatementKind.UsingStatement, new UsingParselet())
            .RegisterStatementParselet(StatementKind.PrintStatement, new PrintParselet());
    }
}