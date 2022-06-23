using System.Collections.Generic;
using BeanLib.References;
using UnityEngine;

/// <summary>
/// A Component holding a collection of categorized audio clips.
/// </summary>
public class ClipCollection : ReferenceResolvedBehaviour
{
    [AutoReference] private AudioManager audioManager;

    [SerializeField] private List<ClipCategory> categories;

    private Dictionary<string, ClipCategory> categoryClips = new Dictionary<string, ClipCategory>();

    /// <summary>
    /// Tries to play a random clip belonging to a specific category.
    /// </summary>
    /// <param name="category">The category to play.</param>
    /// <param name="position">The position to play the sound at.</param>
    public void PlayCategory(string category, Vector2 position)
    {
        // check if a category of that type exists.
        if (categoryClips.ContainsKey(category))
        {
            List<ClipVolume> clips = categoryClips[category].Clips;

            if (clips.Count == 0)
            {
                Debug.LogWarning($"Clip category \"{category}\" is empty");
                return;
            }

            int index = Mathf.RoundToInt((clips.Count - 1) * Random.value);
            ClipVolume clip = clips[index];

            audioManager.Play(clip.Clip, position, clip.Volume * categoryClips[category].GlobalVolume);

            return;
        }

        Debug.LogWarning($"Clip category \"{category}\" does not exist");
    }

    /// <summary>
    /// Tries to play a random clip belonging to a specific category.
    /// </summary>
    /// <param name="category">The category to play.</param>
    public void PlayCategory(string category)
    {
        PlayCategory(category, transform.position);
    }

    private void OnValidate()
    {
        if (categoryClips == null)
        {
            categoryClips = new Dictionary<string, ClipCategory>();
        }

        if (categories == null)
        {
            categories = new List<ClipCategory>();
            return;
        }

        categoryClips.Clear();

        foreach (ClipCategory category in categories)
        {
            categoryClips.Add(category.Category, category);
        }
    }
}