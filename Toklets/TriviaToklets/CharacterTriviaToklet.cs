public sealed class CharacterTriviaToklet : ITriviaToklet<CharacterTriviaToken>
{
    public readonly char character;

    public Predicate<IConsumer<char>> Condition => _condition;
    private Predicate<IConsumer<char>> _condition;

    public CharacterTriviaToklet(char c) {
        character = c;
        _condition = (input => input.Consume() == character);
    }

    public CharacterTriviaToken Consume(IConsumer<char> input, Tokenizer tokenizer) {
        var currChar = input.Consume();

        if (currChar != character) {
            throw Logger.Fatal(new InvalidCallError(ErrorArea.Tokenizer, input.Position));
        }

        return new CharacterTriviaToken(currChar, input.Position);
    }
}