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
    public int samplesPerCollider = 3;

    [Header("Connection Settings")]
    public float maxDirectConnectionDistance = 5f;
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
                RaycastHit2D hit = Physics2D.Raycast(
                    samplePoint + Vector2.up * 0.1f, // start from a small offset above
                    Vector2.down, 
                    2f, 
                    groundLayerMask
                );

                if (hit.collider != null)
                {
                    // Place the node a little above the surface so linecasts won't collide
                    float nodeVerticalOffset = 0.05f;
                    Vector2 nodePos = hit.point + Vector2.up * nodeVerticalOffset;
                    graph.nodes.Add(new Node(nodePos));
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

    }

    bool IsDirectlyConnected(Node a, Node b)
    {
        float distance = Vector2.Distance(a.position, b.position);
        float verticalDiff = Mathf.Abs(a.position.y - b.position.y);

        if (distance > maxDirectConnectionDistance || verticalDiff > maxDirectVerticalDifference)
            return false;

        if (IsObstructed(a.position, b.position))
            return false;

        return true;
    }

    bool IsJumpFeasible(Node a, Node b)
    {
        if (IsObstructed(a.position, b.position))
            return false;

        float horizontalDistance = Mathf.Abs(a.position.x - b.position.x);
        float verticalDifference = b.position.y - a.position.y;
        Debug.Log("positions:" + a.position + "" + b.position + " Horizontal distance: " + horizontalDistance + " Vertical difference: " + verticalDifference);
        if (horizontalDistance > maxJumpHorizontalDistance)
            return false;
        if (verticalDifference > maxJumpVerticalDifference || verticalDifference < -2.5f)
            return false;
        return true;
    }

    bool IsObstructed(Vector2 start, Vector2 end)
    {
        LayerMask combinedMask = wallLayerMask | groundLayerMask;
        RaycastHit2D hit = Physics2D.Linecast(start, end, combinedMask);
        return hit.collider != null;
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
