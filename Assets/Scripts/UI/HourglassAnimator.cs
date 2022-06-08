using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BeanLib.References;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Helper functions for animations used by the Hourglass.
/// </summary>
[RequireComponent(typeof(Animator))]
public class HourglassAnimator : ReferenceResolvedBehaviour
{
    [AutoReference] private TimeResourceManager timeManager = null;
    private Animator animator;

    [Header("References")]
    [SerializeField] private RectTransform shakeTransform;

    [Header("Colors")]
    [SerializeField] private Color sandColor = Color.white;
    [SerializeField] private Color hurtColor = Color.white;
    [SerializeField] private Color healColor = Color.white;

    [Header("Targets")]
    [SerializeField] private Image[] images;
    [SerializeField] private RawImage[] rawImages;

    [Header("Shake")]
    [SerializeField] private float shakeScale = 2f;

    /// <summary>
    /// Plays the hurt animation.
    /// </summary>
    public void OnHurt()
    {
        animator.Play("Hurt", -1, 0);
    }

    /// <summary>
    /// Plays the heal animation.
    /// </summary>
    public void OnHeal()
    {
        animator.Play("Heal", -1, 0);
    }

    /// <summary>
    /// Sets the colour on all targets.
    /// </summary>
    /// <param name="color">The colour to use.</param>
    public void SetColor(Color color)
    {
        images.Execute((image) => image.color = color);
        rawImages.Execute((image) => image.color = color);
    }

    /// <summary>
    /// Animation helper function
    /// Sets color to sand.
    /// </summary>
    public void ANIMFUNC_SetColorSand()
    {
        SetColor(sandColor);
    }

    /// <summary>
    /// Animation helper function
    /// Sets color to hurt.
    /// </summary>
    public void ANIMFUNC_SetColorHurt()
    {
        SetColor(hurtColor);
    }

    /// <summary>
    /// Animation helper function
    /// Sets color to heal.
    /// </summary>
    public void ANIMFUNC_SetColorHeal()
    {
        SetColor(healColor);
    }

    /// <summary>
    /// Animation helper function
    /// Gives the animation root a random offset according to the shake values.
    /// </summary>
    public void ANIMFUNC_Shake()
    {
        float shakeX = UnityEngine.Random.value * shakeScale;
        float shakeY = UnityEngine.Random.value * shakeScale;
        shakeTransform.anchoredPosition = new Vector2(shakeX, shakeY);
    }

    /// <summary>
    /// Animation helper function
    /// Resets the animation root to its default position.
    /// </summary>
    public void ANIMFUNC_ShakeReset()
    {
        shakeTransform.anchoredPosition = Vector2.zero;
    }

    /// <inheritdoc/>
    public override void Start()
    {
        RunResolve();

        animator = GetComponent<Animator>();

        timeManager.TimeRemoved += (amount) => OnHurt();
        timeManager.TimeAdded += (amount) => OnHeal();
    }
}