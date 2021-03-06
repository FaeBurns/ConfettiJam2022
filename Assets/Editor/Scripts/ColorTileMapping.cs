using System;
using UnityEngine;
using UnityEngine.Tilemaps;

/// <summary>
/// Stores a mapping between a color and a tile.
/// </summary>
[Serializable]
public class ColorTileMapping
{
    /// <summary>
    /// Initializes a new instance of the <see cref="ColorTileMapping"/> class.
    /// </summary>
    /// <param name="color">The color to store.</param>
    /// <param name="tile">The tile to store.</param>
    public ColorTileMapping(Color32 color, TileBase tile)
    {
        Color = color;
        Tile = tile;
    }

    /// <summary>
    /// Gets or Sets the color stored in this mapping.
    /// </summary>
    [field: SerializeField]
    public Color32 Color { get; set; }

    /// <summary>
    /// Gets or Sets the tile stored in this mapping.
    /// </summary>
    [field: SerializeField]
    public TileBase Tile { get; set; }
}