using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using Unity.Mathematics;
using Unity.Jobs;
using Unity.Burst;
using Unity.Collections;

public class Pathfinder : MonoBehaviour
{
    public struct Node
    {
        public Vector2Int coord;
        public Vector2Int parent;
        public int gScore;
        public int hScore;
    }

    Hashtable obstacles;
    Node start, end;
    int safeGuard = 1000;
    // Start is called before the first frame update
    void Start()
    {
        obstacles = new Hashtable();
        start = new Node { coord = Vector2Int.zero, parent = Vector2Int.zero, gScore = int.MaxValue, hScore = int.MaxValue };
        end = new Node { coord = Vector2Int.zero, parent = Vector2Int.zero, gScore = int.MaxValue, hScore = int.MaxValue };
    }

    // Update is called once per frame

    public List<Vector2Int> FindPath(Vector2Int startCoord, Vector2Int endCoord, HashSet<Vector2Int> obs)
    {
        this.start.coord = new Vector2Int(startCoord.x, startCoord.y);
        this.end.coord = new Vector2Int(endCoord.x, endCoord.y);
        NativeHashMap<Vector2Int, bool> isObstacle =
            new NativeHashMap<Vector2Int, bool>(obstacles.Count, Allocator.TempJob);
        NativeHashMap<Vector2Int, Node> nodes =
            new NativeHashMap<Vector2Int, Node>(safeGuard, Allocator.TempJob);
        NativeHashMap<Vector2Int, Node> openSet =
            new NativeHashMap<Vector2Int, Node>(safeGuard, Allocator.TempJob);
        NativeArray<Vector2Int> offsets = new NativeArray<Vector2Int>(8, Allocator.TempJob);

        foreach (Vector2Int o in obs)
        {
            isObstacle.Add(o, true);
        }

        AStar aStar = new AStar
        {
            isObstacle = isObstacle,
            offsets = offsets,
            nodes = nodes,
            openSet = openSet,
            start = start,
            end = end,
            safeGuard = safeGuard
        };

        JobHandle handle = aStar.Schedule();
        handle.Complete();
        List<Vector2Int> path = new List<Vector2Int>();
        Vector2Int currentCoord = new Vector2Int(end.coord.x, end.coord.y);
        if (nodes.ContainsKey(end.coord))
        {
            path.Add(currentCoord);
            while (!currentCoord.Equals(start.coord))
            {
                currentCoord = nodes[currentCoord].parent;
                Vector2Int currentTile = new Vector2Int(currentCoord.x,
                    currentCoord.y);
                path.Add(currentTile);
            }
        }
        else
        {
            nodes.Dispose();
            openSet.Dispose();
            isObstacle.Dispose();
            offsets.Dispose();
            return null;
        }
        nodes.Dispose();
        openSet.Dispose();
        isObstacle.Dispose();
        offsets.Dispose();

        path.Reverse();
        return path;
    }

    [BurstCompile(CompileSynchronously = true)]
    public struct AStar : IJob
    {
        public NativeHashMap<Vector2Int, bool> isObstacle;
        public NativeHashMap<Vector2Int, Node> nodes;
        public NativeHashMap<Vector2Int, Node> openSet;
        public NativeArray<Vector2Int> offsets;

        public Node start;
        public Node end;

        public int safeGuard;

        public void Execute()
        {
            Node current = start;
            current.gScore = 0;
            current.hScore = SquaredDistance(current.coord, end.coord);
            openSet.TryAdd(current.coord, current);

            offsets[0] = new Vector2Int(0, 1);
            offsets[1] = new Vector2Int(1, 1);
            offsets[2] = new Vector2Int(1, 0);
            offsets[3] = new Vector2Int(1, -1);
            offsets[4] = new Vector2Int(0, -1);
            offsets[5] = new Vector2Int(-1, -1);
            offsets[6] = new Vector2Int(-1, 0);
            offsets[7] = new Vector2Int(-1, 1);

            int counter = 0;
            do
            {
                current = openSet[ClosestNode()];
                nodes.TryAdd(current.coord, current);

                for (int i = 0; i < offsets.Length; i++)
                {
                    if (!nodes.ContainsKey(current.coord + offsets[i]) &&
                        !isObstacle.ContainsKey(current.coord + offsets[i]))
                    {
                        Node neighbour = new Node
                        {
                            coord = current.coord + offsets[i],
                            parent = current.coord,
                            gScore = current.gScore +
                                SquaredDistance(current.coord, current.coord + offsets[i]),
                            hScore = SquaredDistance(current.coord + offsets[i], end.coord)
                        };

                        if (openSet.ContainsKey(neighbour.coord) && neighbour.gScore <
                            openSet[neighbour.coord].gScore)
                        {
                            openSet[neighbour.coord] = neighbour;
                        }
                        else if (!openSet.ContainsKey(neighbour.coord))
                        {
                            openSet.TryAdd(neighbour.coord, neighbour);
                        }
                    }
                }

                openSet.Remove(current.coord);
                counter++;

                if (counter > safeGuard)
                    break;

            } while (openSet.Count() != 0 && !current.coord.Equals(end.coord));
        }
        public int SquaredDistance(Vector2Int coordA, Vector2Int coordB)
        {
            int a = coordB.x - coordA.x;
            int b = coordB.y - coordA.y;
            return a * a + b * b;
        }

        public Vector2Int ClosestNode()
        {
            Node result = new Node();
            int fScore = int.MaxValue;

            NativeArray<Node> nodeArray = openSet.GetValueArray(Allocator.Temp);

            for (int i = 0; i < nodeArray.Length; i++)
            {
                if (nodeArray[i].gScore + nodeArray[i].hScore < fScore)
                {
                    result = nodeArray[i];
                    fScore = nodeArray[i].gScore + nodeArray[i].hScore;
                }
            }

            nodeArray.Dispose();
            return result.coord;
        }
    }
}