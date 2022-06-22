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

    [BindComponent(Child = true)] private SpriteRenderer gfx;
    [BindComponent(Parent = true)] private CollectibleActivator activator;

    [SerializeField] private float timeReward = 30f;

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

            Destroy(particleObj);
            Destroy(gameObject.transform.parent.gameObject);
        }
    }
}
