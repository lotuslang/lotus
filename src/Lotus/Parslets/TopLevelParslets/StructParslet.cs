namespace Lotus.Syntax;

public sealed class StructParslet : ITopLevelParslet<StructNode>
{
    public static readonly StructParslet Instance = new();

    private static readonly TupleParslet<ExpressionParser, ValueNode, StructField> _fieldsParslet
        = new(ParseStructField) {
            Start = "{",
            End = "}",
            Delim = ";",
            In = "in a struct field list",
            EndingDelimBehaviour = TupleEndingDelimBehaviour.Force
        };

    private static StructField ParseStructField(ExpressionParser parser) {
        var isValid = true;

        if (!parser.TryConsume<IdentNode>(out var name, out var nameNode)) {
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

        var typeOrDefaultResult = parser.TryConsumeEither<NameNode, OperationNode>(out var typeNameNode);

        static bool isTypeOrDefaultValid(Union<NameNode, OperationNode> union) {
            return union.Match(
                name => true,
                op => op is { OperationType: OperationType.Assign, Operands.Length: 2 } &&
                        op.Operands[0] is NameNode
            );
        }

        var isOpValid = true;

        if (!typeOrDefaultResult.Match(
            isTypeOrDefaultValid,
            static () => false
        )) {
            Logger.Error(new UnexpectedError<ValueNode>(ErrorArea.Parser) {
                Value = typeNameNode,
                As = "a field's type",
                Expected = "a type name"
            });

            isValid = false;
            isOpValid = false;
        }

        var typeOrDefault = typeOrDefaultResult.Match(un => un, static () => NameNode.NULL);

        var (typeName, equalSign, defaultValue) = typeOrDefault.Match(
            static (name) => (name, Token.NULL, ValueNode.NULL),
            (op) => (   // we already checked that op[0] is NameNode
                isOpValid ? (NameNode)op.Operands[0] : NameNode.NULL,
                isOpValid ? op.Token : Token.NULL,
                isOpValid ? op.Operands[1] : ValueNode.NULL) // we already check the length
        );

        return new StructField(name, typeName, defaultValue, equalSign) { IsValid = isValid };
    }

    public StructNode Parse(TopLevelParser parser, Token structToken) {
        Debug.Assert(structToken == "struct");

        var name = parser.ConsumeTypeDeclarationName();

        var fields = _fieldsParslet.Parse(parser.ExpressionParser);

        return new StructNode(structToken, name, fields) { IsValid = name.IsValid && fields.IsValid };
    }
}