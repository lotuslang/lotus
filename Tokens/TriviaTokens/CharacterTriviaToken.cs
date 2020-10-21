public class CharacterTriviaToken : TriviaToken
{
    public char Character { get; }

    public CharacterTriviaToken(char c, LocationRange location, bool isValid = true, TriviaToken? leading = null, TriviaToken? trailing = null)
        : base(c.ToString(), TriviaKind.character, location, isValid, leading, trailing)
    {
        Character = c;
    }
}