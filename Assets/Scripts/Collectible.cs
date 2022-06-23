using System;
using BeanLib.References;
using UnityEngine;

/// <summary>
/// Component responsible for collecting items.
/// </summary>
public class Collectible : ReferenceResolvedBehaviour
{
    private GameObject particleObj;

    [AutoReference] private CollectibleGoalManager goalManager;
    [AutoReference] private TimeResourceManager timeManager;

    [BindComponent] private ClipCollection audioCollection;
    [BindComponent(Child = true)] private SpriteRenderer gfx;
    [BindComponent(Parent = true)] private CollectibleActivator activator;

    [SerializeField] private float timeReward = 30f;

    /// <summary>
    /// Event triggered when this <see cref="Collectible"/> is collected.
    /// </summary>
    public event Action<Sprite> OnCollect;

    /// <inheritdoc/>
    public override void Start()
    {
        base.Start();

        OnCollect += goalManager.OnCollect;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Player") && activator.CanUse)
        {
            OnCollect(gfx.sprite);

            timeManager.Add(timeReward, false);

            audioCollection.PlayCategory("Collect");

            Destroy(particleObj);
            Destroy(gameObject.transform.parent.gameObject);
        }
    }
}
