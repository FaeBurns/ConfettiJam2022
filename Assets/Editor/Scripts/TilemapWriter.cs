using System.Collections.Generic;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Tilemaps;

public class TilemapWriter
{
    private readonly Tilemap tilemap;

    public TilemapWriter(Tilemap tilemap)
    {
        this.tilemap = tilemap;
    }

    public void Parse(Dictionary<TileBase, Vector2Int[]> tilePositions)
    {
        Debug.Log("Beginning write to tilemap");

        tilemap.ClearAllTiles();

        foreach (KeyValuePair<TileBase, Vector2Int[]> pair in tilePositions)
        {
            foreach (Vector2Int position in pair.Value)
            {
                WritePosition(position, pair.Key);
            }
        }

        Debug.Log("Write complete");

        EditorSceneManager.MarkSceneDirty(SceneManager.GetActiveScene());
    }

    private void WritePosition(Vector2Int tilePosition, TileBase tile)
    {
        tilemap.SetTile(new Vector3Int(tilePosition.x, tilePosition.y, 0), tile);
    }
}
