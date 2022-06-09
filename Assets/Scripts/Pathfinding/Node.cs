using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

public class Node
{
    public int G { get; set; }
    public int H { get; set; }
    public int F { get; set; }
    public Node Parent { get; set; }
    public Vector2Int Position { get; set; }
    public float CostMultiplier { get; set; }

    public Node(Vector2Int position)
    {
        this.Position = position;
    }
}