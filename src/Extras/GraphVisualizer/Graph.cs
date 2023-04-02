using System.Collections.ObjectModel;

namespace Lotus.Extras.Graphs;

[DebuggerDisplay("{name} ({RootNodes.Count} root nodes)")]
public sealed class Graph : IEquatable<Graph>
{
    public string ID { get; }
    private readonly int _numericID;

    /// <summary>
    /// The name of this graph.
    /// </summary>
    public string Name { get; set; }

    private readonly List<GraphNode> _rootNodes = new();
    /// <summary>
    /// The nodes that are the roots of independent trees.
    /// </summary>
    public ReadOnlyCollection<GraphNode> RootNodes => _rootNodes.AsReadOnly();

    private readonly List<Graph> _rootClusters = new();
    public ReadOnlyCollection<Graph> RootClusters => _rootClusters.AsReadOnly();

    public Dictionary<string, string> GraphProps { get; } = new();

    public Dictionary<string, string> NodeProps { get; } = new();

    public Dictionary<string, string> EdgeProps { get; } = new();

    public Graph(int id, string name) {
        _numericID = id;
        ID = _numericID.ToString(System.Globalization.CultureInfo.InvariantCulture);
        Name = name;

        SetGraphProp("compound", "true");
        SetGraphProp("splines", "line");
    }

    public Graph(string name) : this(Random.Shared.Next(), name) { }

    /// <summary>
    /// Adds a new node to this Graph as a root of an independent tree.
    /// </summary>
    /// <param name="node">The GraphNode to add.</param>
    public void AddNode(GraphNode node)
        => _rootNodes.Add(node);

    public void AddCluster(Graph graph)
        => _rootClusters.Add(graph);

    public void Add(GraphNode node) => AddNode(node);
    public void Add(Graph graph) => AddCluster(graph);

    public void AppendTo(IndentedStringBuilder sb, HashSet<GraphNode> nodeRegistry, HashSet<Graph> graphRegistry, bool isRoot) {
        if (isRoot)
            sb.Append("graph ").AppendLine(ID);
        else
            sb.Append("subgraph cluster_").AppendLine(ID);

        sb.AppendLine('{');

        sb.Indent++;

        sb.Append("label=\"").Append(Name).AppendLine('"');

        foreach (var property in GraphProps) {
            sb.Append(property.Key).Append("=\"").Append(property.Value).Append('"');
            sb.AppendLine();
        }

        if (NodeProps.Count != 0) {
            sb.Append("node [");

            sb.Indent++;
            sb.AppendLine();

            foreach (var property in NodeProps) {
                sb.Append(property.Key).Append("=\"").Append(property.Value).Append('"');
                sb.AppendLine();
            }

            sb.Indent--;
            sb.AppendLine(']');
        }

        if (EdgeProps.Count != 0) {
            sb.Append("edge [");

            sb.Indent++;
            sb.AppendLine();

            foreach (var property in EdgeProps) {
                sb.Append(property.Key).Append("=\"").Append(property.Value).Append('"');
                sb.AppendLine();
            }

            sb.Indent--;
            sb.AppendLine(']');
        }

        foreach (var node in _rootNodes) {
            // If the node wasn't already processed
            if (nodeRegistry.Add(node))
               node.AppendTo(sb, nodeRegistry, graphRegistry);
        }

        foreach (var cluster in _rootClusters) {
            if (graphRegistry.Add(cluster))
                cluster.AppendTo(sb, nodeRegistry, graphRegistry, false);
        }

        sb.Indent--;

        // Close the graph by a closing curly bracket on a new line
        sb.AppendLine().Append('}')
          .AppendLine().Append("// ").Append(StructuralComparer.GetHashCode(this))
          .AppendLine();
    }

    public string ToText() {
        var sb = new IndentedStringBuilder();
        AppendTo(sb, new(), new(), true);
        return sb.ToString();
    }

    public Graph SetName(string name) {
        Name = name;
        return this;
    }

    private Graph SetProperty(string property, string value, Dictionary<string, string> propDic) {
        if (String.IsNullOrEmpty(property))
            return this;

        if (!propDic.TryAdd(property, value))
            propDic[property] = value;

        return this;
    }

    public Graph SetGraphProp(string property, string value)
        => SetProperty(property, value, GraphProps);

    public Graph SetNodeProp(string property, string value)
        => SetProperty(property, value, NodeProps);

    public Graph SetEdgeProp(string property, string value)
        => SetProperty(property, value, EdgeProps);

    public override int GetHashCode()
        => _numericID;

    public override bool Equals(object? obj)
        => Equals(obj as Graph);
    public bool Equals(Graph? other)
        => other is not null && GetHashCode() == other.GetHashCode();

    public static IEqualityComparer<Graph> StructuralComparer => EqComparer.Instance;

    private sealed class EqComparer : EqualityComparer<Graph>
    {
        public static readonly EqComparer Instance = new();

        private EqComparer() {}

        public override bool Equals(Graph? g1, Graph? g2)
            => g1 is null
                ? g2 is null
                : g2 is not null && GetHashCode(g1) == GetHashCode(g2);

        public override int GetHashCode(Graph graph) {
            var code = new DeterministicHashCode();

            foreach (var node in graph.RootNodes)
                code.Add(node, GraphNode.StructuralComparer);

            foreach (var cluster in graph.RootClusters)
                code.Add(cluster, this);

            foreach (var prop in graph.GraphProps.Concat(graph.NodeProps).Concat(graph.EdgeProps)) {
                code.Add(prop.Key, DeterministicStringComparer.Instance);
                code.Add(prop.Value, DeterministicStringComparer.Instance);
            }

            return code.ToHashCode();
        }
    }
}