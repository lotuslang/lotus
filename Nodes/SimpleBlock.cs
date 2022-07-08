using System.Collections.ObjectModel;

public record SimpleBlock
{
    public static readonly SimpleBlock NULL = new(Array.Empty<StatementNode>(), Token.NULL, Token.NULL, false);

    public ReadOnlyCollection<StatementNode> Content { get; }

    public LocationRange Location { get; init; }

    public bool IsValid { get; set; }

    public bool IsOneLiner { get; }

    public Token OpeningToken { get; init; }

    public Token ClosingToken { get; init; }

    public int Count => Content.Count;

    public SimpleBlock(IList<StatementNode> content, Token openingToken, Token closingToken, bool isValid = true)
        : this(content, new LocationRange(openingToken.Location, closingToken.Location), openingToken, closingToken, isValid) { }

    public SimpleBlock(IList<StatementNode> content, LocationRange location, Token openingToken, Token closingToken, bool isValid = true) {
        Content = content.AsReadOnly();
        Location = location;
        OpeningToken = openingToken;
        ClosingToken = closingToken;
        IsValid = isValid;
        IsOneLiner = false;
    }

    public SimpleBlock(StatementNode content, LocationRange location, bool isValid = true)
        : this(new[] { content }, location, Token.NULL, Token.NULL, isValid)
    {
        IsOneLiner = true;
    }

    [DebuggerHidden()]
    [DebuggerStepThrough()]
    [DebuggerNonUserCode()]
    [System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
    public T Accept<T>(IStatementVisitor<T> visitor) => visitor.Visit(this);
}