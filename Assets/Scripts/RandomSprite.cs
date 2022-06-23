using BeanLib.References;
using UnityEngine;

public class RandomSprite : ReferenceResolvedBehaviour
{
    [BindComponent] private SpriteRenderer spriteRenderer;

    [SerializeField] private Sprite[] sprites;

    [SerializeField] private float spawnChance = 0.2f;


    public override void Start()
    {
        base.Start();

        if (Random.value > spawnChance)
        {
            Destroy(gameObject);
        }

        spriteRenderer.sprite = sprites[Mathf.RoundToInt((sprites.Length - 1) * Random.value)];
    }
}