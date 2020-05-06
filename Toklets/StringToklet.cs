using System;

public sealed class StringToklet : IToklet<ComplexToken>
{
    public Predicate<IConsumer<char>> Condition
        => (input => input.Consume() == '\'' || input.Current == '"');

    public ComplexToken Consume(IConsumer<char> input, Tokenizer tokenizer) {

        var endingDelimiter = input.Consume();

        // consume a character
        var currChar = input.Consume();

        // the output token
        var output = new ComplexToken("", TokenKind.@string, input.Position);

        // while the current character is not the ending delimiter
        while (currChar != endingDelimiter) {

            // add it to the value of output
            output.Add(currChar);

            if (!input.Consume(out currChar)) {
                throw new UnexpectedEOF("in string", "a character or a delimiter (' or \")", input.Position);
            }
        }

        // return the output token
        return output;
    }
}