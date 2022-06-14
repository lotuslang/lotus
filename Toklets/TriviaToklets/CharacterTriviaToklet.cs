
public sealed class CharacterTriviaToklet : ITriviaToklet<CharacterTriviaToken>
{
    public char Character { get; }

    public CharacterTriviaToklet(char c) {
        Character = c;
        _condition = (input => input.Consume() == Character);
    }

    public Predicate<IConsumer<char>> Condition => _condition;
	private Predicate<IConsumer<char>> _condition;

    public CharacterTriviaToken Consume(IConsumer<char> input, Tokenizer tokenizer) {
        var currChar = input.Consume();

        if (currChar != Character) {
            throw Logger.Fatal(new InvalidCallError(ErrorArea.Tokenizer, input.Position));
        }

        return new CharacterTriviaToken(currChar, input.Position);
    }
}