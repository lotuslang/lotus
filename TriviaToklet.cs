using System;
using System.Text;
using System.Collections.Generic;

public class TriviaToklet : Toklet
{
    protected override Predicate<IConsumer<char>> condition
        => (_ => false);
}

public class CommentTriviaToklet : TriviaToklet
{
    protected override Predicate<IConsumer<char>> condition
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

    public override Token Consume(IConsumer<char> input, Tokenizer tokenizer) {

        var commentStart = input.Position;

        var currChar = input.Consume();

        if (currChar != '/') throw new Exception();

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

                    strBuilder.Append(inner[inner.Count - 1].Representation);

                    continue;
                }

                strBuilder.Append(currChar);
            }

            if (input.Current == '\0' || input.Current == '\u0003') {
                throw new Exception("unexpected eof in comment");
            }

            // consumes the remaining '/'
            input.Consume();

            strBuilder.Append("*/");

            return new CommentTriviaToken(strBuilder.ToString(), commentStart, inner: inner, trailing: tokenizer.ConsumeTrivia());
        }

        throw new Exception();
    }
}

public class WhitespaceTriviaToklet : TriviaToklet
{
    protected override Predicate<IConsumer<char>> condition
        => (input =>  {

                if (input.Consume() == '\n') return false;

                if (Char.IsWhiteSpace(input.Current)) return true;

                return false;
            }
        );

    public override Token Consume(IConsumer<char> input, Tokenizer tokenizer) {
        var startingPosition = input.Position;

        var whitespaceChar = input.Consume();

        int charCounter = 1;

        while (input.Consume(out char currChar) && currChar == whitespaceChar) charCounter++;

        input.Reconsume();

        return new WhitespaceTriviaToken(whitespaceChar, charCounter, startingPosition, trailing: tokenizer.ConsumeTrivia());
    }
}

public class NewlineTriviaToklet : TriviaToklet
{
    protected override Predicate<IConsumer<char>> condition
        => (input => input.Consume() == '\n');

    public override Token Consume(IConsumer<char> input, Tokenizer tokenizer) {
        var startingPosition = input.Position;

        int charCounter = 0;

        while (input.Consume() == '\n') charCounter++;

        input.Reconsume();

        return new NewlineTriviaToken(charCounter, startingPosition, trailing: tokenizer.ConsumeTrivia());
    }
}