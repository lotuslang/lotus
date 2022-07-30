public abstract record NameNode(Token Token, ImmutableArray<IdentToken> Parts, bool IsValid = true)
: ValueNode(
    Token,
    Parts.Length == 0
        ? Token.Location
        : new LocationRange(Parts.First().Location, Parts.Last().Location),
    IsValid
)
{
    public new static readonly NameNode NULL = IdentNode.NULL;

    private Lazy<OperatorToken> _opToken = new Lazy<OperatorToken>(
        () => Token as OperatorToken ?? new OperatorToken(Token.Representation, Precedence.Access, true, Token.Location, IsValid)
    );

    public OperationNode AsOperation()
        => new OperationNode(
            _opToken.Value,
            Parts.Cast<ValueNode>().ToArray(),
            OperationType.Access,
            IsValid
        );

    [DebuggerHidden()]
    [DebuggerStepThrough()]
    [DebuggerNonUserCode()]
    [System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
    public override T Accept<T>(IValueVisitor<T> visitor) => visitor.Visit(this);
}
