public abstract class StatementVisitor<T>
{
    protected abstract T Default(StatementNode node);

    protected abstract T Default(ValueNode node);



    public virtual T Visit(StatementNode node) => Default(node);

    public virtual T Visit(BreakNode node) => Default(node);
    public virtual T Visit(ContinueNode node) => Default(node);
    public virtual T Visit(DeclarationNode node) => Default(node);
    public virtual T Visit(ElseNode node) => Default(node);
    public virtual T Visit(ForeachNode node) => Default(node);
    public virtual T Visit(ForNode node) => Default(node);
    public virtual T Visit(FromNode node) => Default(node);
    public virtual T Visit(FunctionDeclarationNode node) => Default(node);
    public virtual T Visit(IfNode node) => Default(node);
    public virtual T Visit(ImportNode node) => Default(node);
    //TODO: public virtual T Visit(NamespaceNode node) => Default(node);
    public virtual T Visit(PrintNode node) => Default(node);
    public virtual T Visit(ReturnNode node) => Default(node);
    public virtual T Visit(StatementExpressionNode node) => Default(node as StatementNode);
    public virtual T Visit(UsingNode node) => Default(node);
    public virtual T Visit(WhileNode node) => Default(node);


    public virtual T Visit(ValueNode node) => Default(node);

    public virtual T Visit(BoolNode node) => Default(node);
    public virtual T Visit(ComplexStringNode node) => Default(node);
    public virtual T Visit(FunctionCallNode node) => Default(node);
    public virtual T Visit(IdentNode node) => Default(node);
    public virtual T Visit(NumberNode node) => Default(node);
    public virtual T Visit(ObjectCreationNode node) => Default(node);
    public virtual T Visit(OperationNode node) => Default(node);
    public virtual T Visit(ParenthesizedValueNode node) => Visit(node as TupleNode);
    public virtual T Visit(StringNode node) => Default(node);
    public virtual T Visit(TupleNode node) => Default(node);

    public abstract T Visit(SimpleBlock block);
}