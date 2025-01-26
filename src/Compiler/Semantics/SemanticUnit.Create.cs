using Lotus.Semantics.Binding;
using Lotus.Syntax;

namespace Lotus.Semantics;

public partial class SemanticUnit
{
    internal SymbolFactory Factory { get; set; }

    private SemanticUnit() {
        Factory = new(this);
        Global = new NamespaceInfo("<global>", this);
        InitAndAddSpecialTypes(Global);
    }

    public SemanticUnit(IEnumerable<SyntaxTree> trees) : this() {
        foreach (var tree in trees)
            IsValid &= TryAddTree(tree);

        IsValid &= BindDefinitions();
    }

    // we first want to add the declarations to the scope
    // and then fill-in (resolve) the actual type of everything.
    // To implement that, we have a list of pending "fillings"
    // that need to be done; each time we process a new declaration,
    // we just queue up an action that will be executed after every
    // declaration has been added to the scope. only then can we start
    // resolving type names and stuff
    private List<Func<bool>> _bindingCallbacks = [];
    private bool BindDefinitions() {
        bool isValid = true;

        foreach (var callback in _bindingCallbacks)
            isValid &= callback();

        return isValid;

    }

    private bool TryAddTree(SyntaxTree tree) {
        var ns = GetOrAddTreeNamespace(tree);
        Console.WriteLine($"Tree for file '{tree.Location.filename}' is in namespace {ns}");

        var importedScope = GetImportedScope(tree);

        var fileScope = Scope.Combine(importedScope, Scope.From(ns), GlobalScope);

        var isValid = true;

        // todo: as a slight enhancement, maybe we can already fill-in
        // the field names (or value names for enums) during the initial
        // declaration, but we'll live the resolving of types and default
        // values and such to later.

        foreach (var node in tree.TopNodes) {
            switch (node) {
                case EnumNode enumNode:
                    var enumType = Factory.GetEmptyEnumSymbol(enumNode);
                    isValid &= ns.TryAdd(enumType);
                    _bindingCallbacks.Add(() => {
                        Factory.BindEnumSymbol(enumType, enumNode, fileScope);
                        return enumType.IsValid;
                    });
                    break;
                case StructNode structNode:
                    var structType = Factory.GetEmptyStructSymbol(structNode);
                    isValid &= ns.TryAdd(structType);
                    _bindingCallbacks.Add(() => {
                        Factory.BindStructSymbol(structType, structNode, fileScope);
                        return structType.IsValid;
                    });
                    break;
                case FunctionDefinitionNode funcDefNode:
                    var func = Factory.GetEmptyFunctionSymbol(funcDefNode.Header);
                    isValid &= ns.TryAdd(func);
                    _bindingCallbacks.Add(() => {
                        Factory.BindFunctionSymbol(func, funcDefNode.Header, fileScope);
                        return func.IsValid;
                    });
                    break;
                case TraitNode traitNode:
                    // todo(sem): process trait nodes in TryAddTree
                    break;
                default:
                    // the other nodes are just imports, usings, and namespaces
                    // so we can just ignore those for now
                    Debug.Assert(node is ImportNode or NamespaceNode or UsingNode, $"Can't process {node.GetType().Name}");
                    continue;
            }
        }

        return isValid;
    }

    private Scope GetImportedScope(SyntaxTree tree) {
        var usingScopes
            = tree
                .TopNodes
                .OfType<UsingNode>()
                .Select(GetNamespaceForUsing)
                .Select(ns =>
                    ns is null ? Scope.Empty : Scope.From(ns)
                );

        var combinedUsings = Scope.Combine(usingScopes!);

        var imports = tree.TopNodes.OfType<ImportNode>();
        // todo: `from foo import bar, zab` needs specific/synthetic scope
        var combinedImports = Scope.Empty;

        return Scope.Combine(combinedUsings, combinedImports);
    }

    private NamespaceInfo? GetNamespaceForUsing(UsingNode usingNode) {
        var symbol = usingNode.Name.Match(
            strName => throw new NotImplementedException("using from file not implemented yet"),
            qualifiedName => GlobalScope.ResolveQualified(qualifiedName)
        );

        if (symbol is NamespaceInfo ns)
            return ns;

        Logger.Error(new UnknownSymbolError(ErrorArea.Semantics) {
            ExpectedKinds = [ "namespace" ],
            SymbolName = usingNode.Name.Match(s => s.Value, n => n.ToFullString()),
            In = "using statement",
            Location = usingNode.Location
        });

        return null;
    }

    private NamespaceInfo GetOrAddTreeNamespace(SyntaxTree tree) {
        var nsDecl = tree.TopNodes.OfType<NamespaceNode>().SingleOrDefault();

        if (nsDecl is null)
            return Global;

        var currNs = Global;
        foreach (var part in nsDecl.Name.Parts) {
            // we don't use ResolveQualified because we want to continue when "failing"
            if (Scope.From(currNs).Get(part) is not NamespaceInfo nextNs) {
                nextNs = new NamespaceInfo(part, this);
                _ = currNs.TryAdd(nextNs);
            }

            currNs = nextNs;
        }

        return currNs;
    }
}