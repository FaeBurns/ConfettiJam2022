using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Handles the display of the time left in the <see cref="TimeResourceManager"/>.
/// </summary>
public class HourglassDisplay : MonoBehaviour
{
    [SerializeField] private TimeResourceManager timeManager;

    [SerializeField] private RectTransform spentTransform;
    [SerializeField] private RectTransform leftTransform;

    [SerializeField] private Color sandColor;

    [SerializeField] private float spent;
    [SerializeField] private float left;

    private void Update()
    {
        
    }
}
