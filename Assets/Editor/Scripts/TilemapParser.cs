using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class TilemapParser
{
    private readonly Texture2D texture;
    private readonly ColorTileMapping[] mapping;
    private readonly Tilemap tilemap;
    private readonly TileBase unkownColorTile;

    /// <summary>
    /// Initializes a new instance of the <see cref="TilemapParser"/> class.
    /// </summary>
    /// <param name="texture">The texture to parse.</param>
    /// <param name="mapping">The tile to colour mapping to use.</param>
    /// <param name="tilemap">The tilemap to write to.</param>
    /// <param name="unkownColorTile">The tile to use for unknown colours.</param>
    public TilemapParser(Texture2D texture, ColorTileMapping[] mapping, Tilemap tilemap, TileBase unkownColorTile)
    {
        this.texture = texture;
        this.mapping = mapping;
        this.tilemap = tilemap;
        this.unkownColorTile = unkownColorTile;
    }

    /// <summary>
    /// Parse the texture and write to the tilemap.
    /// </summary>
    public void Parse()
    {
        TileImageReader reader = new TileImageReader(texture);
        Dictionary<Color32, List<Vector2Int>> colorPositions = reader.Parse();

        Dictionary<TileBase, Vector2Int[]> tilePositions = GetTilePositions(colorPositions);

        TilemapWriter writer = new TilemapWriter(tilemap);
        writer.Parse(tilePositions);
    }

    private Dictionary<TileBase, Vector2Int[]> GetTilePositions(Dictionary<Color32, List<Vector2Int>> colorPositions)
    {
        Debug.Log("Converting to positions");

        Dictionary<Color32, TileBase> colorsToTiles = GetMappingDictionary();
        Dictionary<TileBase, Vector2Int[]> tilePositions = new Dictionary<TileBase, Vector2Int[]>();

        List<Vector2Int> unkownColorPositions = new List<Vector2Int>();

        foreach (KeyValuePair<Color32, List<Vector2Int>> pair in colorPositions)
        {
            if (colorsToTiles.ContainsKey(pair.Key))
            {
                TileBase tile = colorsToTiles[pair.Key];
                List<Vector2Int> positions = new List<Vector2Int>();
                positions.AddRange(pair.Value);

                if (tilePositions.ContainsKey(tile))
                {
                    positions.AddRange(tilePositions[tile]);
                }

                tilePositions[tile] = positions.ToArray();
            }
            else
            {
                unkownColorPositions.AddRange(pair.Value);
            }
        }

        tilePositions[unkownColorTile] = unkownColorPositions.ToArray();

        Debug.Log("Position conversion complete");

        return tilePositions;
    }

    private Dictionary<Color32, TileBase> GetMappingDictionary()
    {
        Debug.Log("Converting mapping to dictionary");

        Dictionary<Color32, TileBase> result = new Dictionary<Color32, TileBase>();

        foreach (ColorTileMapping map in mapping)
        {
            Color32 targetColor = map.Color;
            targetColor.a = byte.MaxValue;

            result.Add(targetColor, map.Tile);
        }

        Debug.Log("Dictionary conversion complete");

        return result;
    }
}