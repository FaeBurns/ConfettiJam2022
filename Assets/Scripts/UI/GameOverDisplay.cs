using BeanLib.References;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameOverDisplay : ReferenceResolvedBehaviour
{
    [AutoReference] private PlayerHealth playerHealth;

    [BindComponent] private Animator animator;

    [SerializeField] private RectTransform hourglassTransform;
    [SerializeField] private HourglassDisplay hourglassDisplay;

    public override void Start()
    {
        base.Start();

        playerHealth.OnDeath += PlayerHealth_OnDeath;
    }

    public void ButtonRestart()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    public void ButtonMenu()
    {
        SceneManager.LoadScene(0);
    }

    private void PlayerHealth_OnDeath()
    {
        // disable auto update
        hourglassDisplay.enabled = false;

        // force time display to empty
        hourglassDisplay.SetTime(0f);

        animator.Play("GameOver");
    }
}