using System;
using System.Collections.Generic;

public class ForParselet : IStatementParselet
{
    public StatementNode Parse(Parser parser, Token token) {
        if (!(token is ComplexToken forToken && forToken == "for")) {
            throw new Exception();
        }

        var header = new List<StatementNode>();

        if (parser.Tokenizer.Consume() != "(") throw new Exception();

        var commaCount = 0; // FIXME: I feel dirty

        while (parser.Tokenizer.Peek() != ")") {

            if (parser.Tokenizer.Peek() == ",") {
                parser.Tokenizer.Consume();

                commaCount++;

                // add an empty statement
                header.Add(new StatementNode("", new Token('\0', TokenKind.EOF, parser.Position)));

                continue;
            }

            header.Add(parser.Consume());

            if (parser.Tokenizer.Consume() != ",") {

                if (parser.Tokenizer.Current == ")") {
                    parser.Tokenizer.Reconsume();
                    break;
                }

                throw new UnexpectedTokenException(parser.Tokenizer.Current, "in comma-separated list", ",");
            }

            commaCount++;
        }

        parser.Tokenizer.Consume();

        if (commaCount > 2) throw new Exception("too many statements in for-loop header (" + (commaCount + 1) + ")");

        // if there's not enough statements in the header (happens when the last statement in not specified),
        // add an empty statement
        if (header.Count == 2) header.Add(new StatementNode("", new Token('\0', TokenKind.EOF, parser.Position)));

        var body = parser.ConsumeSimpleBlock();

        return new ForNode(forToken, header.ToArray(), body);
    }
}