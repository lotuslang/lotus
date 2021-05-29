using System.IO;
using System.Linq;
using System.Collections.Generic;

public class ExpressionParser : Parser<ValueNode>
{
    public override ValueNode Current {
        get;
        protected set;
    } = ConstantDefault;

    public new static readonly ValueNode ConstantDefault = ValueNode.NULL;

    public override ValueNode Default {
        get {
            var output = ConstantDefault;

            output.Location = Position;

            return output;
        }
    }

    public ExpressionParser(IConsumer<Token> tokenConsumer) : base(tokenConsumer, LotusGrammar.Instance) { }

    public ExpressionParser(IConsumer<ValueNode> nodeConsumer) : base(nodeConsumer, LotusGrammar.Instance) { }

    public ExpressionParser(StringConsumer consumer) : this(new LotusTokenizer(consumer)) { }

    public ExpressionParser(IEnumerable<Token> tokens) : base(tokens, LotusGrammar.Instance) { }

    public ExpressionParser(IEnumerable<char> collection) : this(new LotusTokenizer(collection)) { }

    public ExpressionParser(FileInfo file) : this(new LotusTokenizer(file)) { }

    public ExpressionParser(Parser<ValueNode> parser) : base(parser) { }


    public override ValueNode Peek()
        => new ExpressionParser(this).Consume();

    public override ValueNode[] Peek(int n) {
        var parser = new ExpressionParser(this);

        var output = new List<ValueNode>();

        for (int i = 0; i < n; i++) {
            output.Add(parser.Consume());
        }

        return output.ToArray();
    }

    public override ValueNode Consume() {
        base.Consume();

        return ConsumeValue(Precedence.Comma);
    }

    public ValueNode ConsumeValue(Precedence precedence = 0) {
        var token = Tokenizer.Consume();

        if (!Grammar.IsPrefix(Grammar.GetExpressionKind(token))) {

            if (token.Kind == TokenKind.EOF) {
                Logger.Error(new UnexpectedEOFException(
                    message: "Encountered an EOF where a value was expected",
                    range: Position
                ));
            } else if (token != ";") {
                Logger.Error(new UnexpectedTokenException(
                    token: token,
                    expected: new[] {
                        TokenKind.@bool,
                        TokenKind.@operator,
                        TokenKind.@string,
                        TokenKind.complexString,
                        TokenKind.identifier,
                        TokenKind.number
                    }
                ));
            }

            return new ValueNode(token, Position, false);
        }

        var left = Grammar.GetPrefixParslet(token).Parse(this, token);

        token = Tokenizer.Consume(); // FIXME: Update this to use bool Consume(out Token)

        if (token == Tokenizer.Default) {
            return left;
        }

        if (Grammar.IsPostfix(Grammar.GetExpressionKind(token))) {
            left = Grammar.GetPostfixParslet(token).Parse(this, token, left);

            token = Tokenizer.Consume();
        }

        while (precedence < Grammar.GetPrecedence(token)) {
            left = Grammar.GetOperatorParslet(token).Parse(this, token, left);

            token = Tokenizer.Consume();
        }

        Tokenizer.Reconsume();

        //left.IsValid = isValid;

        return left;
    }

    public TupleNode ConsumeTuple(string start, string end, int expectedItemCount = -1) {
        var startingDelimiter = Tokenizer.Consume();

        var isValid = true;

        if (startingDelimiter.Representation != start) {
            Logger.Warning(new UnexpectedTokenException( // should we use InvalidCallException ?
                token: startingDelimiter,
                context: "in a tuple",
                expected: start
            ));
        }

        var items = new List<ValueNode>();

        var itemCount = 0;

        while (Tokenizer.Peek() != end && Tokenizer.Peek() != Tokenizer.Default) {

            items.Add(ConsumeValue());

            ++itemCount;

            if (Tokenizer.Consume() != ",") {

                if (Tokenizer.Current == end) {
                    Tokenizer.Reconsume();

                    break;
                }

                var lastItem = items.Last();

                if (Tokenizer.Current.Kind == TokenKind.keyword || lastItem.Token.Kind == TokenKind.keyword) {
                    Tokenizer.Reconsume();

                    isValid = false;

                    break;
                }

                if (!isValid || !lastItem.IsValid) {
                    continue;
                }

                if (Tokenizer.Current == "}") {
                    Tokenizer.Reconsume();

                    break;
                }

                Logger.Error(new UnexpectedTokenException(
                    message: "Did you forget a parenthesis or a comma in this tuple ? Expected " + end + " or ','",
                    token: Tokenizer.Current
                ));

                if (Tokenizer.Peek() == ",") {
                    Tokenizer.Consume();
                }

                isValid = false;

                //Tokenizer.Reconsume();

                continue;
            }

            // since we know that there's a comma right before, if there's an ending delimiter right after it,
            // it's an error.
            // Example : (hello, there, friend,) // right here
            //           ----------------------^--------------
            //                      litterally right there
            if (Tokenizer.Peek() == end) {
                Logger.Error(new UnexpectedTokenException(
                    token: Tokenizer.Consume(),
                    context: "in a tuple",
                    expected: "value"
                ));

                isValid = false;

                break;
            }
        }

        var endingToken = Tokenizer.Consume();

        if (isValid && endingToken != end) { // we probably got an EOF
            Logger.Error(new UnexpectedTokenException(
                token: endingToken,
                context: "in a tuple",
                expected: "an ending delimiter '" + end + "'"
            ));

            isValid = false;
        }

        if (isValid && expectedItemCount != -1 && itemCount != expectedItemCount) {

            if (itemCount > expectedItemCount) {
                Logger.Error(new LotusException(
                    message: "There was too many values in this tuple. Expected "
                           + expectedItemCount + ", but got " + itemCount,
                    range: items.Last()?.Token.Location ?? Tokenizer.Position
                ));
            } else {
                Logger.Error(new LotusException(
                    message: "There wasn't enough values in this tuple. Expected "
                           + expectedItemCount + ", but got " + itemCount,
                    range: items.Last()?.Token.Location ?? Tokenizer.Position
                ));
            }

            isValid = false;
        }

        return new TupleNode(items, startingDelimiter, endingToken, isValid);
    }
}