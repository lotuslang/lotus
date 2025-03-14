namespace Lotus.Syntax.Visitors;

public interface ITopLevelVisitor<out T>
{
    T Default(TopLevelNode node);

    T Visit(TopLevelNode node) => Default(node);
    T Visit(FunctionHeaderNode node) => Default(node);
    T Visit(FunctionDefinitionNode node) => Default(node);
    T Visit(ImportNode node) => Default(node);
    T Visit(NamespaceNode node) => Default(node);
    T Visit(UsingNode node) => Default(node);
    T Visit(EnumNode node) => Default(node);
    T Visit(StructNode node) => Default(node);
    T Visit(TraitNode node) => Default(node);
}