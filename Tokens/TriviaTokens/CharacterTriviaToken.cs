public class CharacterTriviaToken : TriviaToken
{
    public char Character { get; }

    public CharacterTriviaToken(char c, LocationRange location, bool isValid = true, TriviaToken? leading = null, TriviaToken? trailing = null)
        : base(c.ToString(), TriviaKind.character, location, isValid, leading, trailing)
    {
        Character = c;
    }

    [System.Diagnostics.DebuggerHidden()]
    [System.Diagnostics.DebuggerStepThrough()]
    [System.Diagnostics.DebuggerNonUserCode()]
    [System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
    public override T Accept<T>(TokenVisitor<T> visitor) => visitor.Visit(this);
}