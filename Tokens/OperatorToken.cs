[System.Diagnostics.DebuggerDisplay("{Location} {Precedence}({(int)Precedence}) : {Representation}")]
public class OperatorToken : Token
{
    public Precedence Precedence { get; protected set; }

    public bool IsLeftAssociative { get; protected set; }


    public OperatorToken(string representation, Precedence precedence, bool isLeftAssociative, Location location, TriviaToken? leading = null, TriviaToken? trailing = null)
        : base(representation, TokenKind.@operator, location, leading, trailing)
    {
        Precedence = precedence;
        IsLeftAssociative = isLeftAssociative;
    }

    public OperatorToken(string representation, Precedence precedence, string associativity, Location location, TriviaToken? leading = null, TriviaToken? trailing = null)
        : this(representation, precedence, associativity == "left", location, leading, trailing)
    { }

    public OperatorToken(char representation, Precedence precedence, string associativity, Location location, TriviaToken? leading = null, TriviaToken? trailing = null)
        : this(representation.ToString(), precedence, associativity == "left", location, leading, trailing)
    { }

    public OperatorToken(char representation, Precedence precedence, bool isLeftAssociative, Location location, TriviaToken? leading = null, TriviaToken? trailing = null)
        : this(representation.ToString(), precedence, isLeftAssociative, location, leading, trailing)
    { }

    public OperatorToken(Token token, Precedence precedence, string associativity, Location location, TriviaToken? leading = null, TriviaToken? trailing = null)
        : this(token, precedence, associativity == "left", location, leading, trailing)
    {
        Kind = token.Kind == TokenKind.function ? TokenKind.function : TokenKind.@operator;
    }
}