using System.Collections;
using System.Collections.ObjectModel;

namespace Lotus.Extras.Graphs;

[DebuggerDisplay("{ID}:{Name}")]
public sealed class GraphNode : IEnumerable<GraphNode>, IEquatable<GraphNode>
{
    /// <summary>
    /// The unique identifier for this node.
    /// </summary>
    public string ID { get; }
    private readonly int _numericID;

    /// <summary>
    /// The name of this node.
    /// </summary>
    public string Name { get; set; }

    private readonly List<GraphNode> _childNodes = new();
    /// <summary>
    /// The children of this node, i.e. the nodes this node points to.
    /// </summary>
    public ReadOnlyCollection<GraphNode> ChildNodes => _childNodes.AsReadOnly();

    private readonly List<Graph> _childClusters = new();
    public ReadOnlyCollection<Graph> ChildClusters => _childClusters.AsReadOnly();

    private readonly Dictionary<string, string> _props = new();
    public ReadOnlyDictionary<string, string> Properties => _props.AsReadOnly();

    public GraphNode(string name) : this(Random.Shared.Next(), name) { }

    public GraphNode(int id, string text) {
        _numericID = id;
        ID = _numericID.ToString(System.Globalization.CultureInfo.InvariantCulture.NumberFormat);
        Name = text;
    }

    /// <summary>
    /// Add a child to this node.
    /// </summary>
    /// <param name="node">The node to add as a child.</param>
    public GraphNode Add(GraphNode? node) {
        if (node is not null)
            _childNodes.Add(node);

        return this;
    }

    public GraphNode Add(Graph? cluster) {
        if (cluster is not null)
            _childClusters.Add(cluster);

        return this;
    }

    public GraphNode SetProperty(string property, string value) {
        if (String.IsNullOrEmpty(property))
            return this;

        if (!_props.TryAdd(property, value)) {
            _props[property] = value;
        }

        return this;
    }

    public GraphNode SetColor(string color)
        => SetProperty("color", color);

    public GraphNode SetTooltip(string tooltipText)
        => SetProperty("tooltip", tooltipText);

    public GraphNode SetName(string name) {
        Name = name;
        return this;
    }

    public void AppendTo(IndentedStringBuilder sb, HashSet<GraphNode> nodeRegistry, HashSet<Graph> graphRegistry) {
        sb.AppendLine();

        // Declare the node: Append the id of the node, and set its label to `name`
        sb.Append(ID).Append(" [label=\"").Append(Name).Append('"');

        foreach (var property in _props) {
            sb.Append(',').Append(property.Key).Append("=\"").Append(property.Value).Append('"');
        }

        sb.Append(']');
        sb.AppendLine();

        if (_childNodes.Count == 0 && _childClusters.Count == 0)
            return;

        sb.Indent++;

        AppendOrderEnforcingEdges(sb);

        // For each node that is a children of this object
        foreach (var child in _childNodes) {
            Edge.AppendEdgeBetween(sb, this, child);

            // If this child wasn't already processed, then append its text
            if (nodeRegistry.Add(child)) {
                sb.Append("subgraph cluster_").Append(child.ID).AppendLine('{');
                sb.Indent++;
                sb.AppendLine("style=invis");
                child.AppendTo(sb, nodeRegistry, graphRegistry);
                sb.Indent--;
                sb.AppendLine('}');
            }
        }

        foreach (var cluster in _childClusters) {
            Edge.AppendEdgeBetween(sb, this, cluster);

            if (graphRegistry.Add(cluster))
                cluster.AppendTo(sb, nodeRegistry, graphRegistry, false);
        }

        sb.Indent--;
    }

    private void AppendOrderEnforcingEdges(IndentedStringBuilder sb) {
        sb.Append("{rank=same;");
        foreach (var node in _childNodes) sb.Append(node.ID).Append(';');
        sb.AppendLine("}");

        // this stops *before* the last one!!
        for (int i = 0; i < _childNodes.Count - 1; i++) {
            sb.Append(_childNodes[i].ID).Append(" -- ").Append(_childNodes[i + 1].ID).Append(" [style=invis];");
        }

        sb.AppendLine();
    }

    public override int GetHashCode() => _numericID;

    public IEnumerator<GraphNode> GetEnumerator()
        => ChildNodes.GetEnumerator();

    IEnumerator IEnumerable.GetEnumerator()
        => GetEnumerator();

    public override bool Equals(object? obj)
        => Equals(obj as GraphNode);

    public bool Equals(GraphNode? node)
        => node is not null && GetHashCode() == node.GetHashCode();

    public static IEqualityComparer<GraphNode> IDComparer => EqualityComparer<GraphNode>.Default;
    public static IEqualityComparer<GraphNode> StructuralComparer => EqComparer.Instance;

    private sealed class EqComparer : EqualityComparer<GraphNode>
    {
        public static EqComparer Instance { get; } = new();

        private EqComparer() { }

        public override bool Equals(GraphNode? n1, GraphNode? n2)
            => n1 is null
                ? n2 is null
                : n2 is not null && GetHashCode(n1) == GetHashCode(n2);

        public override int GetHashCode(GraphNode n) {
            var code = new DeterministicHashCode();

            code.Add(n.Name, DeterministicStringComparer.Instance);

            foreach (var node in n.ChildNodes)
                code.Add(GetHashCode(node));

            foreach (var cluster in n.ChildClusters)
                code.Add(cluster, Graph.StructuralComparer);

            return code.ToHashCode();
        }
    }
}