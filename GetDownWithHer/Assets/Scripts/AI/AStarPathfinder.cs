using System.Collections.Generic;
using UnityEngine;
public class AStarPathfinder
{
    public static List<Vector2> FindPath(Vector2 startPos, Vector2 targetPos, NavigationGraph graph)
    {
        Node startNode = FindClosestNode(startPos, graph.nodes);
        Node targetNode = FindClosestNode(targetPos, graph.nodes);
        if (startNode == null || targetNode == null)
            return null;

        List<Node> openSet = new();
        HashSet<Node> closedSet = new();
        Dictionary<Node, float> gScore = new();
        Dictionary<Node, float> fScore = new();
        Dictionary<Node, Node> cameFrom = new();

        openSet.Add(startNode);
        gScore[startNode] = 0f;
        fScore[startNode] = Heuristic(startNode, targetNode);

        while (openSet.Count > 0)
        {
            Node current = openSet[0];
            foreach (Node n in openSet)
            {
                if (fScore.ContainsKey(n) && fScore[n] < fScore[current])
                    current = n;
            }

            if (current == targetNode)
            {
                return ReconstructPath(cameFrom, current);
            }

            openSet.Remove(current);
            closedSet.Add(current);

            foreach (Node neighbor in current.neighbors)
            {
                if (closedSet.Contains(neighbor))
                    continue;
                float tentative_gScore = gScore[current] + Vector2.Distance(current.position, neighbor.position);
                if (!openSet.Contains(neighbor))
                    openSet.Add(neighbor);
                else if (tentative_gScore >= gScore[neighbor])
                    continue;

                cameFrom[neighbor] = current;
                gScore[neighbor] = tentative_gScore;
                fScore[neighbor] = gScore[neighbor] + Heuristic(neighbor, targetNode);
            }
        }
        return null; // No path found.
    }

    static float Heuristic(Node a, Node b)
    {
        return Vector2.Distance(a.position, b.position);
    }

    static List<Vector2> ReconstructPath(Dictionary<Node, Node> cameFrom, Node current)
    {
        List<Vector2> totalPath = new()
        {
            current.position
        };
        while (cameFrom.ContainsKey(current))
        {
            current = cameFrom[current];
            totalPath.Insert(0, current.position);
        }
        return totalPath;
    }

    static Node FindClosestNode(Vector2 pos, List<Node> nodes)
    {
        Node closest = null;
        float minDist = float.MaxValue;
        foreach (Node n in nodes)
        {
            float d = Vector2.Distance(pos, n.position);
            if (d < minDist)
            {
                minDist = d;
                closest = n;
            }
        }
        return closest;
    }
}
