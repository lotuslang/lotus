namespace Lotus.Semantics.Binding;

internal interface IScope {
    internal Scope Scope { get; }
}

internal abstract class Scope : IScope
{
    // todo: add SemUnit field to Scope
    //public abstract SemanticUnit Unit { get; }

    public static readonly Scope Empty = new EmptyScope();
    private sealed class EmptyScope : Scope {
        public override SymbolInfo? Get(string _) => null;
    }

    // fixme: add accessibility checks
    public abstract SymbolInfo? Get(string name);

    Scope IScope.Scope => this;

    public SymbolInfo? ResolveQualified(IEnumerable<string> parts) {
        SymbolInfo? currSymbol = null;

        var currScope = this;
        foreach (var part in parts) {
            currSymbol = currScope.Get(part);

            if (currSymbol is null)
                return null;

            if (currSymbol is IScope { Scope: var nextScope })
                currScope = nextScope;
        }

        return currSymbol;
    }

    public static Scope From(IScope scoper) => scoper.Scope;

    public static Scope Combine(Scope s) => s;
    public static Scope Combine(Scope s1, Scope s2)
        => (s1, s2) switch {
            (EmptyScope, _) => s2,
            (_, EmptyScope) => s1,
            (CombinedScope cs1, CombinedScope cs2) => new CombinedScope(cs1.scopes.AddRange(cs2.scopes)),
            (CombinedScope cs1, _) => new CombinedScope(cs1.scopes.Add(s2)),
            (_, CombinedScope cs2) => new CombinedScope(cs2.scopes.Add(s1)),
            _ => new CombinedScope([s1, s2])
        };

    public static Scope Combine(params IEnumerable<Scope> scopes) {
        if (scopes.TryGetNonEnumeratedCount(out var count)) {
            if (count == 0)
                return Empty;
            if (count == 1)
                return scopes.First();
            // otherwise, we don't care
        }

        var builder = ImmutableArray.CreateBuilder<Scope>();
        foreach (var scope in scopes) {
            if (scope is CombinedScope combined)
                builder.AddRange(combined.scopes);
            else if (scope is EmptyScope)
                continue;
            else
                builder.Add(scope);
        }

        if (builder.Count == 1)
            return builder[0];

        return new CombinedScope(builder.DrainToImmutable());
    }

    // public static Scope Combine(params IEnumerable<IScope> scopes)
    //     => new CombinedScope(scopes.Select(s => s.Scope));
    private sealed class CombinedScope(ImmutableArray<Scope> _scopes) : Scope
    {
        internal readonly ImmutableArray<Scope> scopes = _scopes;
        public override SymbolInfo? Get(string name) {
            foreach (var scope in scopes) {
                if (scope.Get(name) is SymbolInfo found)
                    return found;
            }

            return null;
        }
    }
}