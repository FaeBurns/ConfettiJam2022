using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

[CreateAssetMenu(fileName = "New AstarTile", menuName = "Tiles/AstarTile")]
public class AStarTile : Tile
{
    public float speed = 1f;
    public float costMult = 1f;
    public bool blocking = false;
}
