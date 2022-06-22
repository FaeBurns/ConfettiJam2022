using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

[CreateAssetMenu(fileName = "New AstarTile", menuName = "Tiles/AstarTile")]
public class AStarTile : Tile
{
    public float speed = 1f;
    public float costMult = 1f;
    public bool blocking = false;

    public float prefabChance = 0.2f;
    public GameObject prefab;

    public override bool StartUp(Vector3Int position, ITilemap tilemap, GameObject go)
    {
        return base.StartUp(position, tilemap, go);

        if (Random.value > prefabChance && Application.isPlaying)
        {
            Instantiate(prefab, position, Quaternion.identity);
        }

    }
}
