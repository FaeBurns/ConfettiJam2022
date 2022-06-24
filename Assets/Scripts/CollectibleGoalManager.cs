using System.Collections.Generic;
using System.Linq;
using BeanLib.References;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

/// <summary>
/// Component repsonsible for tracking the collected collectibles and the progress towards the goal.
/// </summary>
public class CollectibleGoalManager : ReferenceResolvedBehaviour
{
    private Vector2 startPosition;
    private GameObject playerObject;

    [AutoReference] private TimeResourceManager timeManager;

    [BindComponent] private Animator animator;
    [BindComponent] private ClipCollection audioCollection;

    [SerializeField] private GameObject showcasePrefab;

    [SerializeField] private RectTransform showcaseParent;
    [SerializeField] private RectTransform finalParent;
    [SerializeField] private RectTransform animatedShowcaseElement;
    [SerializeField] private List<Image> showcaseImages;

    [SerializeField] private Sprite finalSprite;

    [SerializeField] private float lerpAlpha = 0;

    [SerializeField] private int collectedGoal = 4;

    /// <summary>
    /// Shows the collected item in the UI and displays an animation.
    /// </summary>
    /// <param name="collectedSprite">The sprite to display.</param>
    public void OnCollect(Sprite collectedSprite)
    {
        GameObject newElement = Instantiate(showcasePrefab, animatedShowcaseElement);

        Image newImage = newElement.GetComponent<Image>();
        newImage.sprite = collectedSprite;

        if (showcaseImages.Count + 1 == collectedGoal)
        {
            newElement.transform.SetParent(finalParent, true);
            animator.Play("Final");
            ANIMFUNC_MoveAllBack();

            showcaseImages.Add(newImage);
            return;
        }

        showcaseImages.Add(newImage);

        animator.Play("New");
    }

    /// <summary>
    /// Animation Function - set parent.
    /// </summary>
    public void ANIMFUNC_MoveParent()
    {
        showcaseImages.Last().transform.SetParent(showcaseParent, true);
    }

    /// <summary>
    /// Animation Function - move all back to animated element.
    /// </summary>
    public void ANIMFUNC_MoveAllBack()
    {
        foreach (Image image in showcaseImages)
        {
            image.transform.SetParent(animatedShowcaseElement, false);
        }
    }

    /// <summary>
    /// Animation Function - replace the set of images with one whole image.
    /// </summary>
    public void ANIMFUNC_ReplaceWithWhole()
    {
        foreach (Image image in showcaseImages)
        {
            Destroy(image.gameObject);
        }

        Instantiate(showcasePrefab, animatedShowcaseElement).GetComponent<Image>().sprite = finalSprite;
    }

    /// <summary>
    /// Animation Function - disable the player.
    /// </summary>
    public void ANIMFUNC_DisablePlayer()
    {
        timeManager.enabled = false;
        FindObjectOfType<PlayerMovement>().MovementState = PlayerMovementState.Dead;
    }

    /// <summary>
    /// Animation Function - end the game.
    /// </summary>
    public void ANIMFUNC_EndGame()
    {
        SceneManager.LoadScene("EndScene");
    }

    public void ANIMFUNC_PlayCollectSound()
    {
        audioCollection.PlayCategory("Collect", playerObject.transform.position);
    }

    public void ANIMFUNC_PlayConnectSound()
    {
        audioCollection.PlayCategory("Connect", playerObject.transform.position);
    }

    private void Awake()
    {
        ReferenceStore.RegisterReference(this);

        startPosition = animatedShowcaseElement.transform.position;
        playerObject = FindObjectOfType<PlayerMovement>().gameObject;
    }

    private void Update()
    {
        animatedShowcaseElement.transform.position = Vector2.Lerp(startPosition, showcaseParent.transform.position, lerpAlpha);
    }
}