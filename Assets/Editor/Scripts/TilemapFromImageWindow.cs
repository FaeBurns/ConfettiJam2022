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
        if (!AssetDatabase.IsValidFolder("Assets/Editor"))
        {
            AssetDatabase.CreateFolder("Assets", "Editor");
        }

        if (!AssetDatabase.IsValidFolder("Assets/Editor/Settings"))
        {
            AssetDatabase.CreateFolder("Assets/Editor", "Settings");
        }

        AssetDatabase.SaveAssets();

        if (string.IsNullOrEmpty(AssetDatabase.AssetPathToGUID(AssetPath)))
        {
            settings = CreateInstance<TilemapFromImageSettings>();
        }
        else
        {
            settings = AssetDatabase.LoadAssetAtPath<TilemapFromImageSettings>(AssetPath);
        }
    }

    private void OnDisable()
    {
        if (settings != null)
        {
            if (string.IsNullOrEmpty(AssetDatabase.AssetPathToGUID(AssetPath)))
            {
                AssetDatabase.CreateAsset(settings, AssetPath);
            }
        }

        EditorUtility.SetDirty(settings);

        AssetDatabase.SaveAssets();
    }

    private void OnGUI()
    {
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

        ShowUnknownColorTile();

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

        Color32 inputColor = EditorGUILayout.ColorField(GUIContent.none, mapping.Color, true, false, false, GUILayout.MaxWidth(150f));
        TileBase tile = (TileBase)EditorGUILayout.ObjectField(mapping.Tile, typeof(TileBase), false);

        bool shouldRemove = GUILayout.Button(EditorGUIUtility.IconContent("d_Toolbar Minus"), EditorStyles.miniButtonRight, GUILayout.Width(20f));

        EditorGUILayout.EndHorizontal();

        return (new ColorTileMapping(inputColor, tile), shouldRemove);
    }

    private void ShowUnknownColorTile()
    {
        settings.UnknownColorTile = (TileBase)EditorGUILayout.ObjectField(new GUIContent("Tile for unknown colors"), settings.UnknownColorTile, typeof(TileBase), false);
    }

    private void OnGenerateClicked()
    {
        Debug.Log("Generating tilemap");

        // force save
        OnDisable();

        TilemapParser parser = new TilemapParser(settings.SourceImage, settings.Mapping.ToArray(), settings.TargetTilemap, settings.UnknownColorTile);
        parser.Parse();
    }
}