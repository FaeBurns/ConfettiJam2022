using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameSettings : MonoBehaviour
{
    [SerializeField] private float time = 120f;

    // Use this for initialization
    void Start()
    {
        DontDestroyOnLoad(this);

        SceneManager.sceneLoaded += SceneManager_sceneLoaded;
    }

    private void SceneManager_sceneLoaded(Scene scene, LoadSceneMode mode)
    {
        // if not first scene
        if (scene.buildIndex != 0)
        {
            FindObjectOfType<TimeResourceManager>().SetTimeFromSettings(time);
        }
    }

    public void BeginWithTime(float newTime)
    {
        time = newTime;

        SceneManager.LoadScene(1);
    }
}