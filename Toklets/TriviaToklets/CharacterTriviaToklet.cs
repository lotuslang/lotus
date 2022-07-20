public sealed class CharacterTriviaToklet : ITriviaToklet<CharacterTriviaToken>
{
    public readonly char character;

    public ref readonly Func<char, Func<IConsumer<char>>, bool> Condition => ref _condition;
    private Func<char, Func<IConsumer<char>>, bool> _condition;

    public CharacterTriviaToklet(char c) {
        character = c;
        _condition = ((currChar, _) => currChar == character);
    }

    public CharacterTriviaToken Consume(IConsumer<char> input, Tokenizer tokenizer) {
        var currChar = input.Consume();

        Debug.Assert(currChar == character);

        return new CharacterTriviaToken(currChar, input.Position);
    }
}