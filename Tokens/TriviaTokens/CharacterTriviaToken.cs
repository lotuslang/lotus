public sealed record CharacterTriviaToken(char Character, LocationRange Location, bool IsValid = true)
: TriviaToken(Character.ToString(), TriviaKind.character, Location, IsValid)
{
    public new static readonly CharacterTriviaToken NULL = new('\0', LocationRange.NULL, false);

    [DebuggerHidden()]
    [DebuggerStepThrough()]
    [DebuggerNonUserCode()]
    [System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
    public override T Accept<T>(ITokenVisitor<T> visitor) => visitor.Visit(this);
}