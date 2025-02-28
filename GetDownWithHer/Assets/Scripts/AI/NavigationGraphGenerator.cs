using System.Collections.Generic;
using UnityEngine;

#region Navigation Classes

public class Node
{
    public Vector2 position;
    public List<Node> neighbors = new();

    public Node(Vector2 pos)
    {
        position = pos;
    }
}

public class NavigationGraph
{
    public List<Node> nodes = new();
}

#endregion

public class NavigationGraphGenerator : MonoBehaviour
{
    [Header("Level Bounds (World Coordinates)")]
    public Vector2 levelBottomLeft;
    public Vector2 levelTopRight;

    [Header("Ground Settings")]
    public LayerMask groundLayerMask;
    public LayerMask wallLayerMask;
    public int samplesPerCollider = 3; // Sample points per collider

    [Header("Connection Settings")]
    public float maxDirectConnectionDistance = 10f;
    public float maxJumpHorizontalDistance = 3f;
    public float maxJumpVerticalDifference = 2f;
    public float maxDirectVerticalDifference = 0.5f;

    [Header("Debug Settings")]
    public bool drawNodes = true;
    public bool drawConnections = true;
    public Color nodeColor = Color.cyan;
    public Color connectionColor = Color.green;
    public float nodeGizmoRadius = 0.1f;

    [HideInInspector]
    public NavigationGraph graph;

    void Start()
    {
        GenerateNavigationGraph();
    }

    void GenerateNavigationGraph()
    {
        graph = new NavigationGraph();
        Collider2D[] groundColliders = Physics2D.OverlapAreaAll(levelBottomLeft, levelTopRight, groundLayerMask);

        foreach (Collider2D col in groundColliders)
        {
            Bounds bounds = col.bounds;
            for (int i = 0; i < samplesPerCollider; i++)
            {
                float t = (samplesPerCollider > 1) ? (float)i / (samplesPerCollider - 1) : 0.5f;
                float sampleX = Mathf.Lerp(bounds.min.x, bounds.max.x, t);
                Vector2 samplePoint = new(sampleX, bounds.max.y);

                // Verify the sample point is over ground
                RaycastHit2D hit = Physics2D.Raycast(samplePoint + Vector2.up * 0.1f, Vector2.down, 1f, groundLayerMask);
                if (hit.collider != null)
                {
                    graph.nodes.Add(new Node(samplePoint));
                }
            }
        }

        // Connect nodes with edges.
        for (int i = 0; i < graph.nodes.Count; i++)
        {
            Node a = graph.nodes[i];
            for (int j = i + 1; j < graph.nodes.Count; j++)
            {
                Node b = graph.nodes[j];
                if (IsDirectlyConnected(a, b))
                {
                    a.neighbors.Add(b);
                    b.neighbors.Add(a);
                }
                else if (IsJumpFeasible(a, b))
                {
                    a.neighbors.Add(b);
                    b.neighbors.Add(a);
                }
            }
        }

        Debug.Log("Navigation graph generated with " + graph.nodes.Count + " nodes.");
    }

    bool IsDirectlyConnected(Node a, Node b)
    {
        float distance = Vector2.Distance(a.position, b.position);
        float verticalDiff = Mathf.Abs(a.position.y - b.position.y);

        // Basic range checks
        if (distance > maxDirectConnectionDistance || verticalDiff > maxDirectVerticalDifference)
            return false;

        // Check for obstacles between the two nodes
        if (IsObstructed(a.position, b.position))
            return false;

        // If we get here, there's no wall in between, so we can consider them connected.
        return true;
    }

    bool IsObstructed(Vector2 start, Vector2 end)
    {
        // Cast a line between start and end on your ground/wall layer(s).
        RaycastHit2D hit = Physics2D.Linecast(start, end, wallLayerMask);

        // If hit.collider is not null, it means there's something in between (a wall, etc.)
        return (hit.collider != null);
    }

    bool IsJumpFeasible(Node a, Node b)
    {
        float horizontalDistance = Mathf.Abs(a.position.x - b.position.x);
        float verticalDifference = b.position.y - a.position.y; // positive if b is above a
        if (horizontalDistance > maxJumpHorizontalDistance)
            return false;
        if (verticalDifference > maxJumpVerticalDifference)
            return false;
        return true;
    }

    void OnDrawGizmos()
    {
        if (graph == null)
            return;

        foreach (Node n in graph.nodes)
        {
            Gizmos.color = nodeColor;
            Gizmos.DrawSphere(n.position, nodeGizmoRadius);
            if (drawConnections)
            {
                Gizmos.color = connectionColor;
                foreach (Node neighbor in n.neighbors)
                {
                    Gizmos.DrawLine(n.position, neighbor.position);
                }
            }
        }
    }
}
