public class TriviaToklet : ITriviaToklet<TriviaToken>
{
    public ref readonly Func<char, Func<IConsumer<char>>, bool> Condition => ref _condition;
	private static readonly Func<char, Func<IConsumer<char>>, bool> _condition = ((_, _) => false);

    [System.Diagnostics.CodeAnalysis.DoesNotReturn]
    public TriviaToken Consume(IConsumer<char> input, Tokenizer _)
        => throw Logger.Fatal(new InvalidCallError(ErrorArea.Tokenizer, input.Position));
}