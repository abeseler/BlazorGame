using System.Diagnostics.CodeAnalysis;
using System.Drawing;

namespace BlazorWeb.Logic;

public sealed record Node(Point Position)
{
    public float GCost { get; set; } // Cost from start to this node
    public float HCost { get; set; } // Heuristic cost from this node to end
    public float FCost => GCost + HCost;
    public Node? Parent { get; set; }
}

public static class Pathfinder
{
    private static readonly Dictionary<Point, Node> s_openNodes = [];
    private static readonly HashSet<Point> s_closedSet = [];
    private static readonly Point[] s_neighbors = [new(0, 1), new(0, -1), new(1, 0), new(-1, 0)];

    public static bool TryCalculatePath(Point start, Point end, [NotNullWhen(true)] out Stack<Point>? path)
    {
        s_closedSet.Clear();
        s_openNodes.Clear();
        s_openNodes.Add(start, new Node(start)
        {
            GCost = 0,
            HCost = CalculateDistance(start, end),
            Parent = null
        });

        while (s_openNodes.Values.MinBy(n => n.FCost) is { } currentNode)
        {
            s_openNodes.Remove(currentNode.Position);
            s_closedSet.Add((currentNode.Position));

            foreach (var neighborPosition in Neighbors(currentNode.Position))
            {
                if (neighborPosition == end)
                {
                    path = CalculatePath(new(neighborPosition) { Parent = currentNode });
                    return true; // Path found
                }
                if (s_closedSet.Contains((neighborPosition))) continue;

                var tentativeGCost = currentNode.GCost + 1; // Assuming each move costs 1

                if (s_openNodes.ContainsKey(neighborPosition) is false || tentativeGCost < s_openNodes[neighborPosition].GCost)
                {
                    s_openNodes[neighborPosition] = new(neighborPosition)
                    {
                        Parent = currentNode,
                        GCost = tentativeGCost,
                        HCost = CalculateDistance(neighborPosition, end)
                    };
                }
            }
        }

        path = null;
        return false; // No path found
    }

    public static IEnumerable<Point> Neighbors(Point node)
    {
        foreach (var point in s_neighbors)
        {
            var neighbor = new Point(node.X + point.X, node.Y + point.Y);

            if (GameState.Map.IsInBounds(neighbor))
            {
                yield return neighbor;
            }
        }
    }

    private static float CalculateDistance(Point a, Point b) =>
        Math.Abs(a.X - b.X) + Math.Abs(a.Y - b.Y); // Manhattan distance

    private static Stack<Point> CalculatePath(Node endNode)
    {
        Stack<Point> path = [];
        Node? current = endNode;
        while (current is not null)
        {
            path.Push(current.Position);
            current = current.Parent;
        }
        return path;
    }
}
