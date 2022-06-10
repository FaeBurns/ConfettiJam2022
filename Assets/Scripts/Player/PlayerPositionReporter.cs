using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BeanLib.References;
using UnityEngine;
using UnityEngine.Tilemaps;

/// <summary>
/// Component responsible for keeping track of the player's last valid pathfinding position.
/// </summary>
public class PlayerPositionReporter : ReferenceResolvedBehaviour
{
    [AutoReference] private Tilemap tilemap;

    /// <summary>
    /// Gets the player's last valid pathing position.
    /// </summary>
    public Vector2 ValidPathPosition { get; private set; }

    private void Update()
    {
        Vector3Int tilePos = tilemap.WorldToCell(transform.position);
        AStarTile tile = tilemap.GetTile<AStarTile>(tilePos);
        if (tile != null && !tile.blocking)
        {
            ValidPathPosition = transform.position;
        }
    }
}