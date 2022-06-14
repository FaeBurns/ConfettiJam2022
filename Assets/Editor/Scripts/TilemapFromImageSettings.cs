using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Tilemaps;

/// <summary>
/// Scriptable object storing saved data to do with the <see cref="TilemapFromImageWindow"/>.
/// </summary>
public class TilemapFromImageSettings : ScriptableObject
{
    /// <summary>
    /// Gets or Sets the color to tile mapping.
    /// </summary>
    [field: SerializeField]
    public List<ColorTileMapping> Mapping { get; set; } = new List<ColorTileMapping>();

    [field: SerializeField]
    public TileBase UnknownColorTile { get; set; }

    /// <summary>
    /// Gets or Sets the texture to get the tilemap from.
    /// </summary>
    [field: SerializeField]
    public Texture2D SourceImage { get; set; }

    /// <summary>
    /// Gets or Sets the target tilemap.
    /// </summary>
    [field: SerializeField]
    public Tilemap TargetTilemap { get; set; }
}