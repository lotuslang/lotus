[System.Diagnostics.DebuggerDisplay("{Location} {Precedence}({(int)Precedence}) : {Representation}")]
public class OperatorToken : Token
{
    public Precedence Precedence { get; protected set; }

    public bool IsLeftAssociative { get; protected set; }


    public OperatorToken(string representation, Precedence precedence, bool isLeftAssociative, Location location)
        : base(representation, TokenKind.@operator, location)
    {
        Precedence = precedence;
        IsLeftAssociative = isLeftAssociative;
    }

    public OperatorToken(string representation, Precedence precedence, string associativity, Location location)
        : this(representation, precedence, associativity == "left", location)
    { }

    public OperatorToken(char representation, Precedence precedence, string associativity, Location location)
        : this(representation.ToString(), precedence, associativity == "left", location)
    { }

    public OperatorToken(char representation, Precedence precedence, bool isLeftAssociative, Location location)
        : this(representation.ToString(), precedence, isLeftAssociative, location)
    { }

    public OperatorToken(Token token, Precedence precedence, string associativity, Location location)
        : this(token, precedence, associativity == "left", location)
    {
        Kind = token.Kind == TokenKind.function ? TokenKind.function : TokenKind.@operator;
    }
}