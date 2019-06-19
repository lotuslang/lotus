using System;
using System.Text;
using System.Collections.Generic;

[System.Diagnostics.DebuggerDisplay("{name} ({RootNodes.Count} root nodes)")]
public class Graph
{
    protected string name;

    /// <summary>
    /// The name of this graph.
    /// </summary>
    /// <value>A string that represents the name of this graph.</value>
    public string Name {
        get => name;
    }

    protected List<GraphNode> rootNodes;

    /// <summary>
    /// The nodes that are the roots of independent trees.
    /// </summary>
    /// <value>A List of GraphNode containing every root node.</value>
    public List<GraphNode> RootNodes {
        get => rootNodes;
    }

    public Graph(string name)
    {
        this.name = name;
        rootNodes = new List<GraphNode>();
    }

    /// <summary>
    /// Adds a new node to this Graph as a root of an independent tree.
    /// </summary>
    /// <param name="node">The GraphNode to add.</param>
    public void AddNode(GraphNode node) => rootNodes.Add(node);

    /// <summary>
    /// A representation of this graph in the 'dot' language (https://www.graphviz.org/doc/info/lang.html).
    /// </summary>
    /// <returns>A single string of 'dot' representing this entire graph.</returns>
    public string ToText()
    {
        // Create a new string builder
        var strBuilder = new StringBuilder();

        // Append the keyword 'diagraph' followed by the name of the graph, followed, on a new line, by an opening curly bracket
        strBuilder.AppendLine("digraph " + name);
        strBuilder.AppendLine("{");

        // A list of GraphNode to keep track of visited nodes
        var registry = new List<GraphNode>();

        // For each independent tree in this graph
        foreach (var node in rootNodes)
        {
            // Add the root node to the registry
            registry.Add(node);

            // Append the 'dot' representation of this root node to the graph's representation
            strBuilder.AppendLine("\t" + node.ToText(registry));
        }

        // Close the graph by a closing curly bracket on a new line
        strBuilder.AppendLine("\n}");

        // Return the string builder
        return strBuilder.ToString();
    }
}

[System.Diagnostics.DebuggerDisplay("{name}:{label}")]
public class GraphNode
{
    protected string id;

    /// <summary>
    /// The unique identifier for this node.
    /// </summary>
    /// <value>A string representing the ID of this node.</value>
    public string ID {
        get => id;
    }

    protected string name;

    /// <summary>
    /// The name of this node.
    /// </summary>
    /// <value>A string representing the name of this node.</value>
    public string Name {
        get => name;
    }


    protected List<GraphNode> childrens;

    /// <summary>
    /// The childrens of this node, i.e. the nodes this node points to.
    /// </summary>
    /// <value>A list of GraphNode this node is pointing to, (i.e. childrens).</value>
    public List<GraphNode> Childrens {
        get => childrens;
    }

    public GraphNode(string id)
    {
        this.id = id;
        this.name = id;
        childrens = new List<GraphNode>();
    }

    public GraphNode(string id, string name)
    {
        this.id = id;
        this.name = name;
        childrens = new List<GraphNode>();
    }

    /// <summary>
    /// Add a child to this node.
    /// </summary>
    /// <param name="node">The node to add as a child.</param>
    public void AddNode(GraphNode node) => childrens.Add(node);

    public string ToText(List<GraphNode> registry)
    {
        // Create a new string builder
        var strBuilder = new StringBuilder();

        // Declare the node : Append the id of the node, and set its label to `name`
        strBuilder.AppendLine($"{id} [label={name}]");

        // For each node that is a children of this object
        foreach (var child in childrens)
        {
            // Append the connection of this node (this node's id -> child's id)
            strBuilder.AppendLine("\t" + id + " -> " + child.id);

            // If this child hasn't been registered yet
            if (!registry.Contains(child))
            {
                // Register this child
                registry.Add(child);

                // Then append the representation of the child
                strBuilder.Append("\t" + child.ToText(registry));
            }
        }

        // Return the string builder
        return strBuilder.ToString();
    }
}