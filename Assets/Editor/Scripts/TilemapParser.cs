using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class TilemapParser
{
    private readonly Texture2D texture;
    private readonly ColorTileMapping[] mapping;
    private readonly Tilemap tilemap;

    public TilemapParser(Texture2D texture, ColorTileMapping[] mapping, Tilemap tilemap)
    {
        this.texture = texture;
        this.mapping = mapping;
        this.tilemap = tilemap;
    }

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

        foreach (KeyValuePair<Color32, List<Vector2Int>> pair in colorPositions)
        {
            if (colorsToTiles.ContainsKey(pair.Key))
            {
                TileBase tile = colorsToTiles[pair.Key];
                List<Vector2Int> positions = pair.Value;

                if (tilePositions.ContainsKey(tile))
                {
                    positions.AddRange(tilePositions[tile]);
                    tilePositions.Remove(tile);
                }

                tilePositions.Add(tile, pair.Value.ToArray());
            }
        }

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