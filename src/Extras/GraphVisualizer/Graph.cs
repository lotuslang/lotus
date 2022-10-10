using System.Text;

namespace Lotus.Extras.Graphs;

[DebuggerDisplay("{name} ({RootNodes.Count} root nodes)")]
public sealed class Graph
{
    private readonly string name;

    /// <summary>
    /// The name of this graph.
    /// </summary>
    /// <value>A string that represents the name of this graph.</value>
    public ref readonly string Name => ref name;

    private readonly ImmutableArray<GraphNode>.Builder rootNodes;

    /// <summary>
    /// The nodes that are the roots of independent trees.
    /// </summary>
    /// <value>A List of GraphNode containing every root node.</value>
    public ImmutableArray<GraphNode> RootNodes => rootNodes.ToImmutable();

    private readonly Dictionary<string, string> graphProps;

    public ref readonly Dictionary<string, string> GraphProps => ref graphProps;

    private readonly Dictionary<string, string> nodeProps;

    public ref readonly Dictionary<string, string> NodeProps => ref nodeProps;

    private readonly Dictionary<string, string> edgeProps;

    public ref readonly Dictionary<string, string> EdgeProps => ref edgeProps;

    public Graph(string name) {
        this.name = name;
        rootNodes = ImmutableArray.CreateBuilder<GraphNode>();
        graphProps = new Dictionary<string, string>();
        nodeProps = new Dictionary<string, string>();
        edgeProps = new Dictionary<string, string>();
    }

    /// <summary>
    /// Adds a new node to this Graph as a root of an independent tree.
    /// </summary>
    /// <param name="node">The GraphNode to add.</param>
    public void AddNode(GraphNode node)
        => rootNodes.Add(node);

    /// <summary>
    /// A representation of this graph in the 'dot' language (https://www.graphviz.org/doc/info/lang.html).
    /// </summary>
    /// <returns>A single string of 'dot' representing this entire graph.</returns>
    public string ToText() {
        // Create a new string builder
        var strBuilder = new StringBuilder();

        // Append the keyword 'digraph' followed by the name of the graph, followed, on a new line, by an opening curly bracket
        strBuilder.Append("graph ").AppendLine(name);
        strBuilder.AppendLine("{");

        foreach (var property in graphProps) {
            strBuilder.Append('\t').Append(property.Key).Append("=\"").Append(property.Value).AppendLine("\"");
        }

        if (nodeProps.Count != 0) {
            strBuilder.AppendLine("\tnode[");

            foreach (var property in nodeProps) {
                strBuilder.Append("\t\t").Append(property.Key).Append("=\"").Append(property.Value).AppendLine("\"");
            }

            strBuilder.AppendLine("\t]\n");
        }

        if (edgeProps.Count != 0) {
            strBuilder.AppendLine("\tedge [");

            foreach (var property in edgeProps) {
                strBuilder.Append("\t\t").Append(property.Key).Append("=\"").Append(property.Value).AppendLine("\"");
            }

            strBuilder.AppendLine("\t]");
        }

        strBuilder.AppendLine();

        // A list of GraphNode to keep track of visited nodes
        var registry = new HashSet<GraphNode>();

        // For each independent tree in this graph
        foreach (var node in rootNodes) {
            // Add the root node to the registry
            _ = registry.Add(node);

            // Append the 'dot' representation of this root node to the graph's representation
            strBuilder.Append('\t').AppendLine(node.ToText(registry));
        }

        // Close the graph by a closing curly bracket on a new line
        strBuilder.Append("\n}\n//").Append(GetHashCode()).AppendLine();

        // Return the string builder
        return strBuilder.ToString();
    }

    public void AddGraphProp(string property, string value) {
        if (String.IsNullOrEmpty(property) || String.IsNullOrEmpty(value))
            return;
        graphProps[property] = value;
    }

    public void AddNodeProp(string property, string value) {
        if (String.IsNullOrEmpty(property) || String.IsNullOrEmpty(value))
            return;
        nodeProps[property] = value;
    }

    public void AddEdgeProp(string property, string value) {
        if (String.IsNullOrEmpty(property) || String.IsNullOrEmpty(value))
            return;
        edgeProps[property] = value;
    }

    public override int GetHashCode() => GetHashCode(true);

    public int GetHashCode(bool includeProps) {
        var code = new DeterministicHashCode();

        foreach (var node in rootNodes) {
            code.Add(node, GraphNode.StructuralComparer);
        }

        if (includeProps) {
            foreach (var props in new[] { GraphProps, NodeProps, EdgeProps }) {
                foreach (var prop in props) {
                    code.Add(DeterministicStringComparer.Instance.GetHashCode(prop.Key));
                    code.Add(DeterministicStringComparer.Instance.GetHashCode(prop.Value));
                }
            }
        }

        return code.ToHashCode();
    }
}