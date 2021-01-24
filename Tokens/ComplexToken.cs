public class ComplexToken : Token
{
    public new static readonly ComplexToken NULL = new ComplexToken("", TokenKind.delim, LocationRange.NULL);

    public ComplexToken(string representation, TokenKind kind, LocationRange location, bool isValid = true, TriviaToken? leading = null, TriviaToken? trailing = null)
        : base(representation, kind, location, isValid, leading, trailing) { }

    public virtual void Add(char ch)
        => rep += ch;

    public virtual void Add(string str)
        => rep += str;

    [System.Diagnostics.DebuggerHidden()]
    [System.Diagnostics.DebuggerStepThrough()]
    [System.Diagnostics.DebuggerNonUserCode()]
    [System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
    public override T Accept<T>(TokenVisitor<T> visitor) => visitor.Visit(this);
}