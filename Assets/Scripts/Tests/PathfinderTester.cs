using System.Collections.Generic;
using BeanLib.References;
using UnityEngine;

/// <summary>
/// A component responsible for testing the pathfinder.
/// </summary>
public class PathfinderTester : ReferenceResolvedBehaviour
{
    [AutoReference] private Pathfinder pathfinder;

    [Header("Test values")]
    [SerializeField] private int testPathCount = 1000;
    [SerializeField] private int maxPerFrame = 10;

    [Header("Area")]
    [SerializeField] private Vector2 minArea;
    [SerializeField] private Vector2 maxArea;

    [Header("Readonly")]
    [SerializeField] private int testsPerformed = 0;

    /// <inheritdoc/>
    public override void Start()
    {
        base.Start();
    }

    private void Update()
    {
        int i = testsPerformed;
        for (; i < Mathf.Min(testsPerformed + maxPerFrame, testPathCount); i++)
        {
            PerformRandomTest();
        }

        testsPerformed = i;
    }

    private void PerformRandomTest()
    {
        Vector2 startPos = GetRandomPositionInArea();
        Vector2 endPos = GetRandomPositionInArea();

        GameObject tempObj = new GameObject("killme");
        pathfinder.FindPath(startPos, endPos, tempObj, DisplayPath);
        Destroy(tempObj);
    }

    private Vector2 GetRandomPositionInArea()
    {
        float xPos = Mathf.Lerp(minArea.x, maxArea.x, Random.value);
        float yPos = Mathf.Lerp(minArea.y, maxArea.y, Random.value);

        return new Vector2(xPos, yPos);
    }

    private void DisplayPath(Stack<Vector2> path)
    {
        if (path == null)
        {
            Debug.Log("Failed to create path");
            return;
        }

        Vector2 startPos = path.Pop();

        Color color = GetRandomColor();

        while (path.Count > 0)
        {
            // get line values
            Vector2 endPos = path.Pop();

            // draw line
            Debug.DrawLine(startPos, endPos, color, 1000);

            // update start pos
            startPos = endPos;
        }
    }

    private Color GetRandomColor()
    {
        return new Color(Mathf.Round(Random.value), Mathf.Round(Random.value), Mathf.Round(Random.value), 1);
    }
}