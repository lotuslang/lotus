using System.Text;

namespace Lotus.Semantics;

public class SymbolFormatter : ISymbolVisitor<string>
{
    public static string Format(SymbolInfo symbol)
        => symbol.Accept(new SymbolFormatter());

    string Default(SymbolInfo symbol)
        => throw new NotImplementedException(
            "Can't format symbol of type '" + symbol.GetType() + "'"
        );

    string ISymbolVisitor<string>.Visit(SymbolInfo symbol) => Default(symbol);

    string ISymbolVisitor<string>.Visit(NamespaceInfo ns)
        => ns.ContainingNamespace switch {
            null => "<global>",
            { IsTopNamespace: true } => ns.Name,
            var parentNs => Format(parentNs) + "." + ns.Name
        };

    string ISymbolVisitor<string>.Visit(TypeInfo symbol) => Default(symbol);
    string ISymbolVisitor<string>.Visit(ArrayTypeInfo symbol) => Default(symbol);
    string ISymbolVisitor<string>.Visit(UnionTypeInfo symbol) => Default(symbol);
    string ISymbolVisitor<string>.Visit(MissingTypeInfo symbol) => Default(symbol);
    string ISymbolVisitor<string>.Visit(UserTypeInfo symbol) => Default(symbol);

    string ISymbolVisitor<string>.Visit(EnumTypeInfo symbol)
        => symbol.Name;

    string ISymbolVisitor<string>.Visit(EnumValueInfo symbol)
        => Format(symbol.FromEnum) + "." + symbol.Name;

    string ISymbolVisitor<string>.Visit(StructTypeInfo symbol)
        => symbol.Name;

    string ISymbolVisitor<string>.Visit(FieldInfo symbol)
        => Format(symbol.FromStruct) + "." + symbol.Name + ": " + Format(symbol.Type);

    string ISymbolVisitor<string>.Visit(FunctionInfo symbol) => throw new NotImplementedException();
    string ISymbolVisitor<string>.Visit(ParameterInfo symbol)
        => symbol.Name + ": " + Format(symbol.Type);
    string ISymbolVisitor<string>.Visit(LocalInfo symbol)
        => symbol.Name + ": " + Format(symbol.Type);
}