using System;
using BeanLib.References;
using TMPro;
using UnityEngine;

/// <summary>
/// Handles the display of the time left in the <see cref="TimeResourceManager"/>.
/// </summary>
public class HourglassDisplay : ReferenceResolvedBehaviour
{
    [AutoReference] private TimeResourceManager timeManager = null;

    [SerializeField] private RectTransform spentTransform;
    [SerializeField] private RectTransform leftTransform;

    [SerializeField] private TextMeshProUGUI timeText;
    [SerializeField] private TextMeshProUGUI multiplierText;

    [Header("Positions")]
    [SerializeField] private Vector2 spentMinMax;
    [SerializeField] private Vector2 leftMinMax;

    [Header("Format strings")]
    [SerializeField] private string multiplierFormatString;

    public void SetTime(float time)
    {
        float alpha = time / timeManager.MaxTime;

        float spentPos = Mathf.Lerp(spentMinMax.x, spentMinMax.y, alpha);
        float leftPos = Mathf.Lerp(leftMinMax.x, leftMinMax.y, alpha);

        spentTransform.anchoredPosition = new Vector2(0, spentPos);
        leftTransform.anchoredPosition = new Vector2(0, leftPos);

        timeText.text = new TimeSpan(0, 0, Mathf.FloorToInt(time)).ToString("mm\\:ss");

        multiplierText.text = string.Format(multiplierFormatString, timeManager.AdditionMultiplier.ToString("0.00"));
    }

    private void Update()
    {
        SetTime(timeManager.Time);
    }
}
