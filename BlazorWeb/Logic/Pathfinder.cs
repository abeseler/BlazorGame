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
    private static readonly int[] s_dx = [1, -1, 0, 0];
    private static readonly int[] s_dy = [0, 0, 1, -1];

    public static bool FindPathAStar(Point start, Point end, out Stack<Point>? path)
    {
        var startNode = new Node(start)
        {
            GCost = 0,
            HCost = CalculateDistance(start, end),
            Parent = null
        };

        s_openNodes.Clear();
        s_openNodes.Add(start, startNode);


        while (s_openNodes.Count > 0)
        {
            var currentNode = s_openNodes.Values.MinBy(n => n.FCost) ?? s_openNodes.Values.First();

            if (currentNode.Position == end)
            {
                path = CalculatePath(currentNode);
                return true;
            }

            s_openNodes.Remove(currentNode.Position);
            s_closedSet.Add((currentNode.Position));

            foreach (var neighborPosition in Neighbors(currentNode.Position))
            {
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
        for (var i = 0; i < 4; i++)
        {
            var newX = node.X + s_dx[i];
            var newY = node.Y + s_dy[i];

            if (newX >= 0 && newX < GameState.Map.GridSize.Width && newY >= 0 && newY < GameState.Map.GridSize.Height)
            {
                yield return new(newX, newY);
            }
        }
    }

    private static float CalculateDistance(Point a, Point b) =>
        Math.Abs(a.X - b.X) + Math.Abs(a.Y - b.Y); // Manhattan distance

    private static Stack<Point> CalculatePath(Node endNode)
    {
        Stack<Point> path = [];
        Node? current = endNode;
        while (current != null)
        {
            path.Push(current.Position);
            current = current.Parent;
        }
        return path;
    }
}
