using System.Collections.Generic;
using BeanLib.References;
using TMPro;
using UnityEngine;

/// <summary>
/// Displays changes made to the hourglass timer in the form of small indicators above the time text.
/// </summary>
public class RecentTimeChangeDisplay : ReferenceResolvedBehaviour
{
    private readonly List<GameObject> spawned = new List<GameObject>();

    [AutoReference] private TimeResourceManager timeManager = null;

    [SerializeField] private GameObject displayPrefab;

    /// <inheritdoc/>
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
        string sign = amount > 0 ? "+" : string.Empty;
        string text = $"{sign}{amount}";

        textDisplay.text = text;

        textDisplay.color = amount > 0 ? Color.green : Color.red;
    }
}