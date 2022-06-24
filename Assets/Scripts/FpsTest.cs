using UnityEngine;


public class FpsTest : MonoBehaviour
{
    [SerializeField] private int targetFramerate = 40;

    // Use this for initialization
    private void Start()
    {
        UpdateElements();
    }

    private void UpdateElements()
    {
        Application.targetFrameRate = targetFramerate;
        QualitySettings.vSyncCount = 1;

        //UnityEngine.Debug.Log("Updated target fps");
    }
}