using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

/// <summary>
/// Class responsible for finding a path between two locations.
/// </summary>
public class AStar
{
    private readonly Dictionary<Vector2Int, AStarTile> tilemap;

    private readonly HashSet<Node> openList;
    private readonly HashSet<Node> closedList;

    private readonly Dictionary<Vector2Int, Node> allNodes = new Dictionary<Vector2Int, Node>();

    private Node currentNode;

    private Vector2Int startPos;
    private Vector2Int endPos;

    /// <summary>
    /// Initializes a new instance of the <see cref="AStar"/> class.
    /// </summary>
    /// <param name="tilemap">The tilemap to perform the pathfind on.</param>
    /// <param name="startPos">The start position of the path.</param>
    /// <param name="endPos">The end position of the path.</param>
    public AStar(Dictionary<Vector2Int, AStarTile> tilemap, Vector2 startPos, Vector2 endPos)
    {
        this.tilemap = tilemap;

        // round positions to work on tilemap
        this.startPos = ConvertToTileSpace(startPos);
        this.endPos = ConvertToTileSpace(endPos);

        currentNode = GetNode(this.startPos);

        openList = new HashSet<Node>
        {
            // add start node
            currentNode,
        };

        closedList = new HashSet<Node>();
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="AStar"/> class.
    /// </summary>
    /// <param name="tilemap">The tilemap to perform the pathfind on.</param>
    /// <param name="startPos">The start position of the path.</param>
    /// <param name="endPos">The end position of the path.</param>
    public AStar(Dictionary<Vector2Int, AStarTile> tilemap, Vector2Int startPos, Vector2Int endPos)
    {
        this.tilemap = tilemap;

        // round positions to work on tilemap
        this.startPos = ConvertToTileSpace(startPos);
        this.endPos = ConvertToTileSpace(endPos);

        currentNode = GetNode(this.startPos);

        openList = new HashSet<Node>
        {
            // add start node
            currentNode,
        };

        closedList = new HashSet<Node>();
    }

    /// <summary>
    /// Gets a value indicating whether the algorithm has finished finding a path.
    /// </summary>
    public bool FinishedCalculating { get; private set; }

    /// <summary>
    /// Gets the resulting path.
    /// </summary>
    public Stack<Vector2> Path { get; private set; }

    /// <summary>
    /// Converts the input vector into valid a position on the tilemap.
    /// </summary>
    /// <param name="position">The first vector to convert.</param>
    /// <returns>The converted vector.</returns>
    public static Vector2Int ConvertToTileSpace(Vector2 position)
    {
        return new Vector2Int(Mathf.FloorToInt(position.x), Mathf.FloorToInt(position.y));
    }

    /// <summary>
    /// Runs the pathfinding algorithm.
    /// </summary>
    public void RunAlgorithm()
    {
        while (openList.Count > 0 && Path == null)
        {
            RunAlgorithInnerStep();
        }

        FinishedCalculating = true;
    }

    /// <summary>
    /// Runs a single step of the algorithm.
    /// </summary>
    public void RunAlgorithmStep()
    {
        if (openList.Count > 0 && Path == null)
        {
            RunAlgorithInnerStep();
            return;
        }

        FinishedCalculating = true;
    }

    /// <summary>
    /// Runs the algorithm over multiple frames.
    /// </summary>
    /// <param name="maxStepsPerCycle">Maximum steps to take per frame.</param>
    /// <returns>Coroutine return.</returns>
    public IEnumerator RunAlgorithmEnumerated(int maxStepsPerCycle)
    {
        int index = 0;
        while (openList.Count > 0 && Path == null)
        {
            RunAlgorithInnerStep();

            index++;
            if (index >= maxStepsPerCycle)
            {
                // reset index and wait for next frame
                index = 0;
                yield return null;
            }
        }

        FinishedCalculating = true;
    }

    private void RunAlgorithInnerStep()
    {
        List<Node> neighbours = FindNeighbors(currentNode.Position);

        ExamineNeighbors(neighbours, currentNode);

        UpdateCurrentTile(ref currentNode);

        Path = GeneratePath(currentNode);
    }

    private List<Node> FindNeighbors(Vector2Int parentPosition)
    {
        List<Node> neighbors = new List<Node>();

        for (int x = -1; x <= 1; x++)
        {
            for (int y = -1; y <= 1; y++)
            {
                // exclude center
                if (y != 0 || x != 0)
                {
                    // get neighbor position
                    Vector2Int neighborPosition = new Vector2Int(parentPosition.x - x, parentPosition.y - y);

                    // get tile
                    AStarTile tile = GetTile(neighborPosition);

                    if (neighborPosition != startPos && tile && !tile.blocking)
                    {
                        Node neighbor = GetNode(neighborPosition);
                        neighbor.CostMultiplier = tile.costMult;
                        neighbors.Add(neighbor);
                    }
                }
            }
        }

        return neighbors;
    }

    private void ExamineNeighbors(List<Node> neighbors, Node current)
    {
        for (int i = 0; i < neighbors.Count; i++)
        {
            if (!ConnectedDiagonally(current, neighbors[i]))
            {
                continue;
            }

            int gScore = DetermineGScore(neighbors[i].Position, current.Position);

            if (openList.Contains(neighbors[i]))
            {
                if (current.G + gScore < neighbors[i].G)
                {
                    CalcValues(current, neighbors[i], gScore);
                }
            }
            else if (!closedList.Contains(neighbors[i]))
            {
                CalcValues(current, neighbors[i], gScore);

                openList.Add(neighbors[i]);
            }
        }
    }

    private void CalcValues(Node parent, Node neighbor, int cost)
    {
        neighbor.Parent = parent;

        neighbor.G = parent.G + Mathf.RoundToInt(cost * neighbor.CostMultiplier);

        neighbor.H = (Mathf.Abs(neighbor.Position.x - endPos.x) + Mathf.Abs(neighbor.Position.y - endPos.y)) * 10;

        neighbor.F = neighbor.G + neighbor.F;
    }

    private int DetermineGScore(Vector2Int neighbor, Vector2Int current)
    {
        int gScore;

        int x = current.x - neighbor.x;
        int y = current.y - neighbor.y;

        if (Mathf.Abs(x - y) % 2 == 1)
        {
            gScore = 10;
        }
        else
        {
            gScore = 14;
        }

        return gScore;
    }

    private void UpdateCurrentTile(ref Node current)
    {
        openList.Remove(current);

        closedList.Add(current);

        if (openList.Count > 0)
        {
            current = openList.OrderBy(x => x.F).First();
        }
    }

    private Node GetNode(Vector2Int position)
    {
        if (allNodes.ContainsKey(position))
        {
            return allNodes[position];
        }
        else
        {
            Node node = new Node(position);
            allNodes.Add(position, node);
            return node;
        }
    }

    private bool ConnectedDiagonally(Node current, Node neighbor)
    {
        Vector2Int direction = current.Position - neighbor.Position;

        Vector2Int first = new Vector2Int(current.Position.x + (direction.x * -1), current.Position.y);
        Vector2Int second = new Vector2Int(current.Position.x, current.Position.y + (direction.y * -1));

        AStarTile firstTile = GetTile(first);
        AStarTile secondTile = GetTile(second);

        return !((firstTile != null && firstTile.blocking) || (secondTile != null && secondTile.blocking));
    }

    private Stack<Vector2> GeneratePath(Node current)
    {
        if (current.Position == endPos)
        {
            Stack<Vector2> finalPath = new Stack<Vector2>();

            while (current.Position != startPos)
            {
                // push position at center of tile
                finalPath.Push(current.Position + new Vector2(0.5f, 0.5f));

                current = current.Parent;
            }

            return finalPath;
        }

        return null;
    }

    private AStarTile GetTile(Vector2Int position)
    {
        if (tilemap.TryGetValue(position, out AStarTile result))
        {
            return result;
        }

        return null;
    }
}