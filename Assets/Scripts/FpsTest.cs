using System.Collections;
using System.Diagnostics;
using UnityEngine;


public class FpsTest : MonoBehaviour
{
    [SerializeField] private int targetFramerate = 40;

    // Use this for initialization
    void OnValidate()
    {
        UpdateElements();
    }
    private void UpdateElements()
    {
        Application.targetFrameRate = targetFramerate;
        QualitySettings.vSyncCount = 0;

        UnityEngine.Debug.Log("Updated target fps");
    }
}