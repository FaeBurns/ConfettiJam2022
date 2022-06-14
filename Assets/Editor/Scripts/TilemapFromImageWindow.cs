using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.Tilemaps;

/// <summary>
/// Window responsible for handling the importing of a tilemap from an image.
/// </summary>
public class TilemapFromImageWindow : EditorWindow
{
    private const string AssetPath = "Assets/Editor/Settings/TilemapImageImportSettings.asset";
    private TilemapFromImageSettings settings;

    private bool dropdownVisible = false;

    /// <summary>
    /// Shows the window.
    /// </summary>
    [MenuItem("Window/Tilemap From Image")]
    public static void ShowWindow()
    {
        GetWindow<TilemapFromImageWindow>();
    }

    private void OnEnable()
    {
        AssetDatabase.CreateFolder("Assets", "Editor");
        AssetDatabase.CreateFolder("Assets/Editor", "Settings");

        AssetDatabase.SaveAssets();

        if (string.IsNullOrEmpty(AssetDatabase.AssetPathToGUID(AssetPath)))
        {
            settings = CreateInstance<TilemapFromImageSettings>();
        }
        else
        {
            AssetDatabase.LoadAssetAtPath<TilemapFromImageSettings>(AssetPath);
        }
    }

    private void OnDisable()
    {
        if (settings != null)
        {
            AssetDatabase.CreateAsset(settings, AssetPath);
        }

        AssetDatabase.SaveAssets();
    }

    private void OnGUI()
    {
        settings = (TilemapFromImageSettings)EditorGUILayout.ObjectField("Settings", settings, typeof(TilemapFromImageSettings), false);

        settings.SourceImage = (Texture2D)EditorGUILayout.ObjectField("Image Source", settings.SourceImage, typeof(Texture2D), false);

        bool showColorMapping = EditorGUILayout.DropdownButton(new GUIContent("Color mapping"), FocusType.Passive);
        if (showColorMapping)
        {
            dropdownVisible = !dropdownVisible;
        }

        if (dropdownVisible)
        {
            ShowColorMapping();
        }

        settings.TargetTilemap = (Tilemap)EditorGUILayout.ObjectField("Tilemap Override", settings.TargetTilemap, typeof(Tilemap), true);

        if (GUILayout.Button("Build Tilemap"))
        {
            OnGenerateClicked();
        }
    }

    private void ShowColorMapping()
    {
        List<int> removeIndexes = new List<int>();

        for (int i = 0; i < settings.Mapping.Count; i++)
        {
            (ColorTileMapping mapping, bool cancelled) = ShowIndividualColorMap(settings.Mapping[i]);

            if (cancelled)
            {
                removeIndexes.Add(i);
            }
            else
            {
                settings.Mapping[i] = mapping;
            }
        }

        int offset = 0;

        foreach (int index in removeIndexes)
        {
            settings.Mapping.RemoveAt(index - offset);

            offset++;
        }

        if (GUILayout.Button("Add"))
        {
            settings.Mapping.Add(new ColorTileMapping(Color.white, null));
        }
    }

    private (ColorTileMapping Mapping, bool Removed) ShowIndividualColorMap(ColorTileMapping mapping)
    {
        EditorGUILayout.BeginHorizontal();

        Color inputColor = EditorGUILayout.ColorField(GUIContent.none, mapping.Color, true, false, false, GUILayout.MaxWidth(150f));
        TileBase tile = (TileBase)EditorGUILayout.ObjectField(mapping.Tile, typeof(TileBase), false);

        bool shouldRemove = GUILayout.Button(EditorGUIUtility.IconContent("d_Toolbar Minus"));

        EditorGUILayout.EndHorizontal();

        return (new ColorTileMapping(inputColor, tile), shouldRemove);
    }

    private void OnGenerateClicked()
    {
        Debug.Log("Generating tilemap");
    }
}