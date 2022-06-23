using System;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Contains a category id and audio clip collection.
/// </summary>
[CreateAssetMenu(fileName = "Clip Category", menuName = "ScriptableObjects/Clip Category")]
public class ClipCategory : ScriptableObject
{
    /// <summary>
    /// Gets or sets the category for this collection.
    /// </summary>
    [field: SerializeField]
    public string Category { get; set; }

    /// <summary>
    /// Gets or sets a secondary volume multiplier to apply to all clips.
    /// </summary>
    [field: SerializeField]
    public float GlobalVolume { get; set; }

    /// <summary>
    /// Gets or Sets the <see cref="ClipVolume"/> collection.
    /// </summary>
    [field: SerializeField]
    public List<ClipVolume> Clips { get; set; }
}