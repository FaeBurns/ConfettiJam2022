using BeanLib.References;
using UnityEngine;

/// <summary>
/// A component responsible for managing audio.
/// </summary>
public class AudioManager : MonoBehaviour
{
    [SerializeField] private GameObject prefab;

    /// <summary>
    /// Plays an audio clip at the specified location.
    /// </summary>
    /// <param name="clip">The audio clip to play.</param>
    /// <param name="position">The position to play the clip at.</param>
    /// <param name="volume">The volume to play the clip at.</param>
    public void Play(AudioClip clip, Vector2 position, float volume)
    {
        AudioSource source = Instantiate(prefab, position, Quaternion.identity).GetComponent<AudioSource>();

        source.PlayOneShot(clip, volume);
        Destroy(source.gameObject, clip.length + 0.1f);
    }

    private void Awake()
    {
        ReferenceStore.RegisterReference(this);
    }
}