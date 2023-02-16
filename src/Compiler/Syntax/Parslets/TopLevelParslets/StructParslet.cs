namespace Lotus.Syntax;

public sealed class StructParslet : ITopLevelParslet<StructNode>
{
    public static readonly StructParslet Instance = new();

    public StructNode Parse(Parser parser, Token structToken, ImmutableArray<Token> modifiers) {
        Debug.Assert(structToken == "struct");

        var name = parser.ConsumeValue(
            IdentNode.NULL,
            @as: "a type name"
        );

        var fields = _fieldsParslet.Parse(parser);

        return new StructNode(structToken, name, fields, modifiers) { IsValid = name.IsValid && fields.IsValid };
    }

    private static readonly TupleParslet<StructField> _fieldsParslet
        = new(ParseStructField) {
            Start = "{",
            End = "}",
            Delim = ";",
            In = "in a struct field list",
            EndingDelimBehaviour = TrailingDelimiterBehaviour.Required
        };

#pragma warning disable IDE0018 // Variable declaration can be inlined
    private static StructField ParseStructField(Parser parser) {
        var isValid = true;

        var modifiers = parser.ConsumeModifiers();

        if (!parser.TryConsumeValue<IdentNode>(out var name, out var nameNode)) {
            Logger.Error(new UnexpectedError<ValueNode>(ErrorArea.Parser) {
                Value = nameNode,
                As = "a field name",
                Expected = "an identifier"
            });

            isValid = false;

            name = IdentNode.NULL;
        }

        if (parser.Tokenizer.Consume() != ":") {
            Logger.Error(new UnexpectedError<Token>(ErrorArea.Parser) {
                Value = parser.Tokenizer.Current,
                In = "a struct field declaration",
                Expected = "a colon ':'",
            });

            isValid = false;
        }

        // The declaration could be either:
        //      foo: str;            <-- no default value
        //         OR
        //      foo: str = "stuff";  <-- default value
        //
        // If we encounter the first one, then we'll parse a name,
        // so everything's easy.
        //
        // However, if we have the second one, it gets trickier, since
        // the parser will actually understand it as an assignment, and
        // therefore we'll get back an OperationNode.

        var typeNameOrDefaultResult = parser.TryConsumeEitherValues<NameNode, OperationNode>(out var typeNameNode);

        // if it's neither a type name nor an operation node
        if (!typeNameOrDefaultResult.IsOk()) {
            Logger.Error(new UnexpectedError<ValueNode>(ErrorArea.Parser) {
                Value = typeNameNode,
                As = "a field's type",
                Expected = "a type name"
            });

            isValid = false;

            return new StructField(name, NameNode.NULL, modifiers) { IsValid = isValid };
        }

        var typeNameOrDefault = typeNameOrDefaultResult.Value;

        NameNode? typeName;

        // if it's a type name
        if (typeNameOrDefault.Is<NameNode>(out typeName))
            return new StructField(name, typeName, modifiers) { IsValid = isValid };

        // if we get here, we got an OperationNode
        var assignNode = (OperationNode)typeNameOrDefault;

        // ..however, we're not sure yet if it's actually an assignment, or if it's garbage
        // so we check that the node looks like
        //      <name> = <value>
        isValid
            &= assignNode is { OperationType: OperationType.Assign, Operands.Length: 2 }
            && assignNode.Operands[0] is NameNode; // the type name must be a name

        typeName = NameNode.NULL;
        var equalSign = Token.NULL;
        var defaultValue = ValueNode.NULL;

        if (isValid) {
            typeName = (NameNode)assignNode.Operands[0];
            equalSign = assignNode.Token;
            defaultValue = assignNode.Operands[1];
        }

        return new StructField(name, typeName, defaultValue, equalSign, modifiers) { IsValid = isValid };
    }
}