public interface IValueVisitor<T>
{
    T Default(ValueNode node);

    T Visit(ValueNode node) => Default(node);

    T Visit(BoolNode node) => Default(node);
    T Visit(ComplexStringNode node) => Visit(node as StringNode);
    T Visit(FunctionCallNode node) => Default(node);
    T Visit(IdentNode node) => Default(node);
    T Visit(NumberNode node) => Default(node);
    T Visit(ObjectCreationNode node) => Default(node);
    T Visit(OperationNode node) => Default(node);
    T Visit(ParenthesizedValueNode node) => Visit(node as TupleNode);
    T Visit(StringNode node) => Default(node);
    T Visit(TupleNode node) => Default(node);
}