using System.Collections.Generic;
using UnityEngine;

public class TileImageReader
{
    private readonly Texture2D texture;

    public TileImageReader(Texture2D texture)
    {
        this.texture = texture;
    }

    public Dictionary<Color32, List<Vector2Int>> Parse()
    {
        Debug.Log("Parsing image");
        Dictionary<Color32, List<Vector2Int>> result = new Dictionary<Color32, List<Vector2Int>>();

        int width = texture.width;
        int height = texture.height;

        Color32[] pixels = texture.GetPixels32();

        int i = 0;
        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                Color32 pixel = pixels[i];

                if (!result.ContainsKey(pixel))
                {
                    result.Add(pixel, new List<Vector2Int>());
                }

                result[pixel].Add(new Vector2Int(y, x));

                i++;
            }
        }

        Debug.Log("Image parsed");

        return result;
    }
}
