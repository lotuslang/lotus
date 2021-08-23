public record CharacterTriviaToken(char Character, LocationRange Location, bool IsValid = true)
: TriviaToken(Character.ToString(), TriviaKind.character, Location, IsValid)
{
    public new static readonly CharacterTriviaToken NULL = new('\0', LocationRange.NULL, false);

    [System.Diagnostics.DebuggerHidden()]
    [System.Diagnostics.DebuggerStepThrough()]
    [System.Diagnostics.DebuggerNonUserCode()]
    [System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
    public override T Accept<T>(ITokenVisitor<T> visitor) => visitor.Visit(this);
}