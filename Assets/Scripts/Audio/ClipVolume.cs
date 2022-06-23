using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

/// <summary>
/// Container class that matches an audio clip with a volume.
/// </summary>
[Serializable]
public class ClipVolume
{
    /// <summary>
    /// Gets or sets the clip to use.
    /// </summary>
    [field: SerializeField]
    public AudioClip Clip { get; set; }

    /// <summary>
    /// Gets or sets the volume to play the clip at.
    /// </summary>
    [field: SerializeField]
    public float Volume { get; set; } = 1f;
}