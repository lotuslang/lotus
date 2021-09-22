using System;
using System.Text;
using System.Collections.Generic;

public sealed class CommentTriviaToklet : ITriviaToklet<CommentTriviaToken>
{
    public Predicate<IConsumer<char>> Condition
        => (consumer => consumer.Consume() is '/' && (consumer.Consume() is '/' or '*'));

    private CommentTriviaToken Consume(IConsumer<char> input, Tokenizer tokenizer, bool isInner) {

        var startingPosition = input.Position;

        var isValid = true;

        var currChar = input.Consume();

        if (currChar != '/') {
            throw Logger.Fatal(new InvalidCallException(input.Position));
        }

        currChar = input.Consume();

        if (currChar == '/') {

            var strBuilder = new StringBuilder("//");

            while (input.Consume(out currChar) && currChar != '\n') {
                strBuilder.Append(currChar);
            }

            strBuilder.Append('\n');

            return new CommentTriviaToken(
                strBuilder.ToString(),
                new LocationRange(startingPosition, input.Position)
            ) { TrailingTrivia = tokenizer.ConsumeTrivia() };
        }

        if (currChar == '*') {
            var strBuilder = new StringBuilder("/*");

            var inner = new List<CommentTriviaToken>();

            while (input.Consume(out currChar) && !(currChar == '*' && input.Peek() == '/')) {
                if (currChar == '/' && input.Peek() == '*') {

                    // reconsume the '/'
                    input.Reconsume();

                    inner.Add(Consume(input, tokenizer, isInner: true));

                    strBuilder.Append(inner[^1].Representation);

                    continue;
                }

                strBuilder.Append(currChar);
            }

            // if we stopped on an EOF, then something went wrong
            if (input.Current == input.Default) {
                Logger.Error(new UnexpectedEOFException(
                    context: "in a multi-line comment",
                    expected: "the comment delimiter '*/'",
                    new LocationRange(startingPosition, input.Position)
                ));

                isValid = false;
            }

            // consumes the remaining '/'
            input.Consume();

            strBuilder.Append("*/");

            return new CommentTriviaToken(
                strBuilder.ToString(),
                new LocationRange(startingPosition, input.Position),
                inner: inner,
                isValid: isValid
            ) { TrailingTrivia = isInner ? TriviaToken.NULL : tokenizer.ConsumeTrivia() };
        }

        throw Logger.Fatal(new InvalidCallException(new LocationRange(startingPosition, input.Position)));
    }

    public CommentTriviaToken Consume(IConsumer<char> input, Tokenizer tokenizer)
        => Consume(input, tokenizer, isInner: false);
}