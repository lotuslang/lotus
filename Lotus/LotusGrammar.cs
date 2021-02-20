public sealed class LotusGrammar : ReadOnlyGrammar
{
    private readonly Grammar internalGrammar = new Grammar();

    public LotusGrammar() : base() {
        InitializeToklets();
        InitializeTriviaToklets();
        InitializeExpressionKinds();
        InitializeExpressionParslets();
        InitializeStatementKinds();
        InitializeStatementParslets();

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

    private void InitializeExpressionParslets() {

        // values
        internalGrammar
            .RegisterPrefix(ExpressionKind.Number, new NumberLiteralParslet())
            .RegisterPrefix(ExpressionKind.String, new StringLiteralParslet())
            .RegisterPrefix(ExpressionKind.Identifier, new IdentifierParslet())
            .RegisterPrefix(ExpressionKind.Boolean, new BoolLiteralParslet())
            .RegisterPrefix(ExpressionKind.LeftParen, new LeftParenParslet());

        // prefix operators
        internalGrammar
            .RegisterPrefixOperator(ExpressionKind.Plus, OperationType.Positive)
            .RegisterPrefixOperator(ExpressionKind.Minus, OperationType.Negative)
            .RegisterPrefixOperator(ExpressionKind.Not, OperationType.Not)
            .RegisterPrefixOperator(ExpressionKind.Increment, OperationType.PrefixIncrement)
            .RegisterPrefixOperator(ExpressionKind.Decrement, OperationType.PrefixDecrement);

        // misc prefix parslets
        internalGrammar
            .RegisterPrefix(ExpressionKind.Array, new ArrayLiteralParslet())
            .RegisterPrefix(ExpressionKind.New, new ObjectCreationParslet());

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

        // misc infix parslets
        internalGrammar
            .RegisterInfix(ExpressionKind.Array, new ArrayAccessParslet())
            .RegisterInfix(ExpressionKind.LeftParen, new FuncCallParslet())
            .RegisterInfix(ExpressionKind.Ternary, new TernaryOperatorParslet());

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
            .RegisterStatementKind("namespace", StatementKind.NamespaceStatement)
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

    private void InitializeStatementParslets() {
        internalGrammar
            .RegisterStatementParslet(StatementKind.VariableDeclaration, new DeclarationParslet())
            .RegisterStatementParslet(StatementKind.FunctionDeclaration, new FunctionDeclarationParslet())
            .RegisterStatementParslet(StatementKind.ReturnStatement, new ReturnParslet())
            .RegisterStatementParslet(StatementKind.FromStatement, new ImportParslet())
            .RegisterStatementParslet(StatementKind.NamespaceStatement, new NamespaceParslet())
            .RegisterStatementParslet(StatementKind.ForeachLoop, new ForeachParslet())
            .RegisterStatementParslet(StatementKind.ForLoop, new ForParslet())
            .RegisterStatementParslet(StatementKind.IfStatement, new IfParslet())
            .RegisterStatementParslet(StatementKind.WhileStatement, new WhileParslet())
            .RegisterStatementParslet(StatementKind.DoWhileStatement, new DoWhileParslet())
            .RegisterStatementParslet(StatementKind.BreakStatement, new BreakParslet())
            .RegisterStatementParslet(StatementKind.ContinueStatement, new ContinueParslet())
            .RegisterStatementParslet(StatementKind.UsingStatement, new UsingParslet())
            .RegisterStatementParslet(StatementKind.PrintStatement, new PrintParslet());
    }
}