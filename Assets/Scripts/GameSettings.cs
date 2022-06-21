using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>
/// Component responsible for beginning the game via button.
/// </summary>
public class GameSettings : MonoBehaviour
{
    /// <summary>
    /// Gets or sets the amount of time the player should have.
    /// </summary>
    public static float Time { get; set; } = 120f;

    /// <summary>
    /// Begins the game with the specified time.
    /// </summary>
    /// <param name="newTime">The amount of time the player should have.</param>
    public void BeginWithTime(float newTime)
    {
        Time = newTime;

        SceneManager.LoadScene(1);
    }
}