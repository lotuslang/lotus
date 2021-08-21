using System;
using System.Linq;
using System.Collections.Generic;
using System.Text;

public class Toklet : IToklet<Token>
{
    public Predicate<IConsumer<char>> Condition => (_ => true);

    public virtual Token Consume(IConsumer<char> input, Tokenizer _)
        => new(input.Consume(), TokenKind.delimiter, input.Position);
}