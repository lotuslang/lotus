#nullable disable
#pragma warning disable IDE0034

public abstract class TopLevelVisitor<T>
{
    protected abstract T Default(TopLevelNode node);



    public virtual T Visit(TopLevelNode node) => Default(node);
    public virtual T Visit(TopLevelStatementNode node) => Default(node);
    public virtual T Visit(FromNode node) => Default(node);
    public virtual T Visit(ImportNode node) => Default(node);
    public virtual T Visit(NamespaceNode node) => Default(node);
    public virtual T Visit(UsingNode node) => Default(node);


    // TODO: should we use this ?
    //public abstract T Visit(SimpleBlock block);
}