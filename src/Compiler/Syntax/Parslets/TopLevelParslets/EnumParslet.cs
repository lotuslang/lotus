namespace Lotus.Syntax;

public sealed class EnumParslet : ITopLevelParslet<EnumNode>
{
    public static readonly EnumParslet Instance = new();

    private static ValueNode ParseValue(Parser parser) {
        // return parser.TryConsumeEitherValues<IdentNode, OperationNode>(out var asNode)
        //     .Transform(
        //         onOk: nameOrOp
        //             => nameOrOp.Match(
        //                 name => new Result<Union<IdentNode, OperationNode>>(name),
        //                 op => {
        //                     if (op is not OperationNode {
        //                             OperationType: OperationType.Assign,
        //                             Operands: [IdentNode, NumberNode]
        //                     })
        //                         return Result<Union<IdentNode, OperationNode>>.Error;
        //                     return new Result<Union<IdentNode, OperationNode>>(op);
        //                 }
        //             )
        //     )
        //     .Match(
        //         onOk: u => u.Match<ValueNode>(n => n, n => n),
        //         onError: () => {
        //             Logger.Error(new UnexpectedError<ValueNode>(ErrorArea.Parser) {
        //                 Value = null!,
        //                 In = "an enum body",
        //                 As = "an enum field",
        //                 Expected = "an identifier, optionally with an assigned value"
        //             });

        //             return ValueNode.NULL;
        //         }
        //     );

        var val = parser.ConsumeValue();

        // 'foo' form
        if (val is IdentNode)
            return val;

        // 'foo = 42' form
        if (val is OperationNode {
                OperationType: OperationType.Assign,
                Operands: [IdentNode, NumberNode]
        })
            return val;

        Logger.Error(new UnexpectedError<ValueNode>(ErrorArea.Parser) {
            Value = val,
            In = "an enum body",
            As = "an enum field",
            Expected = "an identifier, optionally with an assigned value"
        });

        val.IsValid = false;

        return val;
    }

    private static readonly TupleParslet<ValueNode> _enumValuesParslet
        = new(ParseValue) {
            Start = "{",
            End = "}",
            EndingDelimBehaviour = TrailingDelimiterBehaviour.Accepted,
            In = "an enum declaration"
        };

    public EnumNode Parse(Parser parser, Token enumToken, ImmutableArray<Token> modifiers) {
        /*
        *   Enums, just like a lot of top-lvl stuff, come in all shapes
        *   and sizes, so parsing them is gonna require a bit of caution
        *   I want enums to be able to inherit from another enum's
        *   value, meaning that you have something like this :
        *
        *       @TokenKind.lts
        *       public enum TokenKind {
        *           delim,
        *           value = 6,
        *           trivia,
        *           EOF,
        *       }
        *
        *       @TriviaKind.lts
        *       public enum TokenKind.trivia::TriviaKind {
        *           whitespace,
        *           comment,
        *           character
        *       }
        *
        */

        var name = parser.ConsumeTypeDeclarationName();

        var values = _enumValuesParslet.Parse(parser);

        // make sure that every node is also valid before saying this one is valid
        if (values.IsValid)
            values.IsValid = values.All(n => n.IsValid);

        var isValid = enumToken.IsValid && name.IsValid && values.IsValid;

        return new EnumNode(name, values, enumToken, modifiers) { IsValid = isValid };
    }
}