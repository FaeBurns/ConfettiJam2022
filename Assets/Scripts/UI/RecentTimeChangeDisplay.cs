using System.Collections;
using UnityEngine;
using BeanLib.References;
using System.Collections.Generic;
using TMPro;
using System;

public class RecentTimeChangeDisplay : ReferenceResolvedBehaviour
{
    private readonly List<GameObject> spawned = new List<GameObject>();

    [AutoReference] TimeResourceManager timeManager;

    [SerializeField] private GameObject displayPrefab;

    public override void Start()
    {
        base.Start();

        timeManager.TimeAdded += ChangePositive;
        timeManager.TimeRemoved += ChangeNegative;
    }

    private void ChangeNegative(float amount)
    {
        DisplayTimeChange(-amount);
    }

    private void ChangePositive(float amount)
    {
        DisplayTimeChange(amount);
    }

    private void DisplayTimeChange(float amount)
    {
        amount = Mathf.Round(amount);

        GameObject newObject = Instantiate(displayPrefab, transform);
        TextMeshProUGUI textDisplay = newObject.GetComponentInChildren<TextMeshProUGUI>();

        spawned.Add(newObject);

        // add positive sign if time is being added
        string sign = amount > 0 ? "+" : "";
        string text = $"{sign}{amount}";

        textDisplay.text = text;

        textDisplay.color = amount > 0 ? Color.green : Color.red;
    }
}