using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Handles the display of the time left in the <see cref="TimeResourceManager"/>.
/// </summary>
public class HourglassDisplay : MonoBehaviour
{
    private RectTransform rectTransform;

    // this is all very stinky
    // find a better way to do it
    // pretty sure I have a library I made somewhere for this exact purpose
    private Coroutine runningHurtCoroutine;
    private Coroutine runningHealCoroutine;
    private float hurtCoroutineEndTime;
    private float healCoroutineEndTime;
    private bool shouldCancelHurt = false;
    private bool shouldCancelHeal = false;

    [SerializeField] private TimeResourceManager timeManager;

    [SerializeField] private RectTransform spentTransform;
    [SerializeField] private RectTransform leftTransform;

    [SerializeField] private TextMeshProUGUI timeText;

    [SerializeField] private RawImage trail;

    [Header("Colors")]
    [SerializeField] private Color sandColor;
    [SerializeField] private Color hurtColor;
    [SerializeField] private Color healColor;

    [Header("Positions")]
    [SerializeField] private Vector2 spentMinMax;
    [SerializeField] private Vector2 leftMinMax;

    [Header("Shaking")]
    [SerializeField] private float shakeTime = 0.2f;
    [SerializeField] private float shakeScale = 3f;

    public void OnHurt()
    {
        hurtCoroutineEndTime = Time.time + shakeTime;

        shouldCancelHeal = true;

        if (runningHurtCoroutine == null)
        {
            StartCoroutine(HurtCoroutine());
        }
    }

    public void OnHeal()
    {
        healCoroutineEndTime = Time.time + shakeTime;

        shouldCancelHurt = true;

        if (runningHealCoroutine == null)
        {
            StartCoroutine(HealCoroutine());
        }
    }

    private void Awake()
    {
        SetColor(sandColor);

        rectTransform = GetComponent<RectTransform>();

        timeManager.TimeRemoved += (amount) => OnHurt();
        timeManager.TimeAdded += (amount) => OnHeal();
    }

    private void Update()
    {
        float alpha = timeManager.TimeResource / timeManager.MaxTime;

        float spentPos = Mathf.Lerp(spentMinMax.x, spentMinMax.y, alpha);
        float leftPos = Mathf.Lerp(leftMinMax.x, leftMinMax.y, alpha);

        spentTransform.anchoredPosition = new Vector2(0, spentPos);
        leftTransform.anchoredPosition = new Vector2(0, leftPos);

        timeText.text = new TimeSpan(0, 0, Mathf.FloorToInt(timeManager.TimeResource)).ToString("mm\\:ss");
    }

    private IEnumerator HurtCoroutine()
    {
        Vector2 initialPosition = rectTransform.anchoredPosition;

        SetColor(hurtColor);

        while(Time.time < hurtCoroutineEndTime)
        {
            if (shouldCancelHurt)
            {
                shouldCancelHurt = false;
                break;
            }

            float shakeX = UnityEngine.Random.Range(-shakeScale, shakeScale);
            float shakeY = UnityEngine.Random.Range(-shakeScale, shakeScale);

            rectTransform.anchoredPosition = initialPosition + new Vector2(shakeX, shakeY);

            // skip frame
            yield return null;
        }

        rectTransform.anchoredPosition = initialPosition;
        SetColor(sandColor);
    }

    private IEnumerator HealCoroutine()
    {
        SetColor(healColor);

        while (Time.time < healCoroutineEndTime)
        {

            if (shouldCancelHeal)
            {
                shouldCancelHeal = false;
                break;
            }

            // skip frame
            yield return null;
        }
        SetColor(sandColor);
    }

    private void SetColor(Color color)
    {
        spentTransform.GetComponent<Image>().color = color;
        leftTransform.GetComponent<Image>().color = color;
        trail.color = color;
    }
}
