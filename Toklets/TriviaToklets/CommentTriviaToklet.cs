using System;
using System.Text;
using System.Collections.Generic;

public sealed class CommentTriviaToklet : ITriviaToklet<CommentTriviaToken>
{
    public Predicate<IConsumer<char>> Condition
        => (consumer => {
                if (consumer.Consume() != '/') {
                    return false;
                }

                if (consumer.Consume() != '/' && consumer.Current != '*') {
                    return false;
                }

                return true;
            }
        );

    public CommentTriviaToken Consume(IConsumer<char> input, Tokenizer tokenizer) {

        var commentStart = input.Position;

        var currChar = input.Consume();

        if (currChar != '/') {
            throw new InternalErrorException("while processing a comment", "because this method shouldn't have been called on this input", input.Position);
        }

        currChar = input.Consume();

        if (currChar == '/') {

            var strBuilder = new StringBuilder("//");

            while (input.Consume(out currChar) && currChar != '\n') {
                strBuilder.Append(currChar);
            }

            strBuilder.Append('\n');

            return new CommentTriviaToken(strBuilder.ToString(), commentStart, trailing: tokenizer.ConsumeTrivia());
        }

        if (currChar == '*') {
            var strBuilder = new StringBuilder("/*");

            var inner = new List<CommentTriviaToken>();

            while (input.Consume(out currChar) && !(currChar == '*' && input.Peek() == '/')) {
                if (currChar == '/' && input.Peek() == '*') {

                    // reconsume the '/'
                    input.Reconsume();

                    inner.Add(Consume(input, tokenizer) as CommentTriviaToken);

                    strBuilder.Append(inner[^1].Representation);

                    continue;
                }

                strBuilder.Append(currChar);
            }

            if (input.Current == '\0' || input.Current == '\u0003') {
                throw new InvalidInputException(strBuilder.ToString(), "in a multi-line comment", "the comment delimiter '*/'", input.Position);
            }

            // consumes the remaining '/'
            input.Consume();

            strBuilder.Append("*/");

            return new CommentTriviaToken(strBuilder.ToString(), commentStart, inner: inner, trailing: tokenizer.ConsumeTrivia());
        }

        throw new InternalErrorException("while processing a comment", "because this method shouldn't have been called on this input", input.Position);
    }
}