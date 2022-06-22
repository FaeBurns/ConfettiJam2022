using BeanLib.References;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;


public class TipRandomizer : ReferenceResolvedBehaviour
{
    private static List<string> possibleTips = new List<string>();

    [BindComponent] private TextMeshProUGUI textMesh;

    [SerializeField] private string prefixText = "Tip: ";
    [SerializeField] private string[] allTips;

    public static void Reset()
    {
        possibleTips.Clear();
    }

    public override void Start()
    {
        base.Start();

        ChoseTip();
    }

    private void ChoseTip()
    {
        if (possibleTips.Count == 0)
        {
            possibleTips.AddRange(allTips);
        }

        string chosenTip = possibleTips[0];
        possibleTips.RemoveAt(0);

        textMesh.text = prefixText + chosenTip;
    }
}