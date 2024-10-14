using System.Collections.Generic;
using UnityEngine;

public class AStarPathfinding : MonoBehaviour
{
    public class Node
    {
        public Vector2Int Position;
        public Node Parent;
        public float GCost; // Cost from start to this node
        public float HCost; // Heuristic cost from this node to end
        public float FCost => GCost + HCost; // Total cost

        public Node(Vector2Int position)
        {
            Position = position;
        }
    }

    public List<Vector2Int> FindPath(Vector2Int start, Vector2Int end, bool[,] walkableMap)
    {
        List<Node> openList = new List<Node>();
        HashSet<Node> closedList = new HashSet<Node>();

        Node startNode = new Node(start);
        Node endNode = new Node(end);

        openList.Add(startNode);

        while (openList.Count > 0)
        {
            Debug.Log("Pathfinding: " + openList.Count);
            Node currentNode = openList[0];
            for (int i = 1; i < openList.Count; i++)
            {
                if (openList[i].FCost < currentNode.FCost || openList[i].FCost == currentNode.FCost && openList[i].HCost < currentNode.HCost)
                {
                    currentNode = openList[i];
                }
            }

            openList.Remove(currentNode);
            closedList.Add(currentNode);

            if (currentNode.Position == endNode.Position)
            {
                return RetracePath(startNode, currentNode);
            }

            foreach (Vector2Int neighborPosition in GetNeighbors(currentNode.Position, walkableMap))
            {
                if (!walkableMap[neighborPosition.x, neighborPosition.y] || closedList.Contains(new Node(neighborPosition)))
                {
                    continue;
                }

                Node neighborNode = new Node(neighborPosition);
                float newMovementCostToNeighbor = currentNode.GCost + GetDistance(currentNode.Position, neighborNode.Position);
                if (newMovementCostToNeighbor < neighborNode.GCost || !openList.Contains(neighborNode))
                {
                    neighborNode.GCost = newMovementCostToNeighbor;
                    neighborNode.HCost = GetDistance(neighborNode.Position, endNode.Position);
                    neighborNode.Parent = currentNode;

                    if (!openList.Contains(neighborNode))
                    {
                        openList.Add(neighborNode);
                    }
                }
            }
        }

        return null; // Path not found
    }

    private List<Vector2Int> RetracePath(Node startNode, Node endNode)
    {
        List<Vector2Int> path = new List<Vector2Int>();
        Node currentNode = endNode;

        while (currentNode != startNode)
        {
            path.Add(currentNode.Position);
            currentNode = currentNode.Parent;
        }

        path.Reverse();
        return path;
    }

    private List<Vector2Int> GetNeighbors(Vector2Int nodePosition, bool[,] walkableMap)
    {
        List<Vector2Int> neighbors = new List<Vector2Int>();

        for (int x = -1; x <= 1; x++)
        {
            for (int y = -1; y <= 1; y++)
            {
                if (x == 0 && y == 0)
                {
                    continue;
                }

                int checkX = nodePosition.x + x;
                int checkY = nodePosition.y + y;

                if (checkX >= 0 && checkX < walkableMap.GetLength(0) && checkY >= 0 && checkY < walkableMap.GetLength(1))
                {
                    neighbors.Add(new Vector2Int(checkX, checkY));
                }
            }
        }

        return neighbors;
    }

    private float GetDistance(Vector2Int a, Vector2Int b)
    {
        int dstX = Mathf.Abs(a.x - b.x);
        int dstY = Mathf.Abs(a.y - b.y);

        if (dstX > dstY)
        {
            return 14 * dstY + 10 * (dstX - dstY);
        }
        return 14 * dstX + 10 * (dstY - dstX);
    }
}