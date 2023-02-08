namespace Lotus.Syntax;

public sealed class EnumParslet : ITopLevelParslet<EnumNode>
{
    public static readonly EnumParslet Instance = new();

    private static readonly ValueTupleParslet<ValueNode> _enumValuesParslet
        = new(static (parser) => parser.Consume()) {
            Start = "{",
            End = "}",
            EndingDelimBehaviour = TupleEndingDelimBehaviour.Accept
        };

    public EnumNode Parse(TopLevelParser parser, Token enumToken, ImmutableArray<Token> modifiers) {
        /*
        *   Enums, just like a lot of top-lvl stuff comes in all shapes
        *   and sizes, so parsing them is gonna require a bit of caution
        *   I want enums to be able to inherit from another enum's
        *   value, meaning that you have something like this :
        *
        *       @TokenKind.lts
        *       public enum TokenKind {
        *           delim,
        *           value,
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

        var values = _enumValuesParslet.Parse(parser.ExpressionParser);

        // check that every field is the right format/type
        foreach (var node in values) {
            if (node is not IdentNode and not OperationNode { OperationType: OperationType.Assign }) {
                Logger.Error(new UnexpectedError<ValueNode>(ErrorArea.Parser) {
                    Value = node,
                    In = "an enum body",
                    As = "an enum field",
                    Expected = "an identifier, maybe with an assigned value"
                });

                values.IsValid = false;
            }
        }

        var isValid = enumToken.IsValid && name.IsValid && values.IsValid;

        return new EnumNode(name, values, enumToken, modifiers) { IsValid = isValid };
    }
}