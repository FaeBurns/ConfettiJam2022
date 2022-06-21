using System;
using BeanLib.References;
using UnityEngine;

/// <summary>
/// Component responsible for collecting items.
/// </summary>
public class Collectible : ReferenceResolvedBehaviour
{
    [AutoReference] private CollectibleGoalManager goalManager;

    [BindComponent(Child = true)] private SpriteRenderer gfx;
    [BindComponent(Parent = true)] private CollectibleActivator activator;

    private event Action<Sprite> OnCollect;

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
            Destroy(gameObject);
        }
    }
}
