public class TriviaToklet : ITriviaToklet<TriviaToken>
{
    public Predicate<IConsumer<char>> Condition => _condition;
	private static readonly Predicate<IConsumer<char>> _condition = (_ => false);

    [System.Diagnostics.CodeAnalysis.DoesNotReturn]
    public TriviaToken Consume(IConsumer<char> input, Tokenizer _)
        => throw Logger.Fatal(new InvalidCallError(ErrorArea.Tokenizer, input.Position));
}