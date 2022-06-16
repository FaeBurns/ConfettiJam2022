using System;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Global extensions class.
/// </summary>
public static class Extensions
{
    /// <summary>
    /// Executes an action on every memeber of a collection.
    /// </summary>
    /// <typeparam name="T">The type of held by the collection.</typeparam>
    /// <param name="collection">The collection to execute on.</param>
    /// <param name="action">The action to run on each member.</param>
    public static void Execute<T>(this IEnumerable<T> collection, Action<T> action)
    {
        foreach (T element in collection)
        {
            action.Invoke(element);
        }
    }

    /// <summary>
    /// Tests to see if two vectors are close enough.
    /// </summary>
    /// <param name="self">this.</param>
    /// <param name="other">The other <see cref="Vector2"/> to compare this to.</param>
    /// <param name="maxDiff">The maximum difference between the vectors.</param>
    /// <returns>True if the vectors are close enough, False if not.</returns>
    public static bool CloseEnough(this Vector2 self, Vector2 other, float maxDiff = 0.01f)
    {
        return Vector2.Distance(self, other) < maxDiff;
    }

    /// <summary>
    /// Rotates the vector around a point by the specified amount.
    /// </summary>
    /// <param name="vector">The vector to rotate.</param>
    /// <param name="center">The center point to rotate around.</param>
    /// <param name="degrees">The amount to rotate by.</param>
    /// <returns>The rotated vector.</returns>
    public static Vector2 Rotate(this Vector2 vector, Vector2 center, float degrees)
    {
        float sin = Mathf.Sin(degrees * Mathf.Deg2Rad);
        float cos = Mathf.Cos(degrees * Mathf.Deg2Rad);

        return new Vector2((cos * vector.x) - (sin * vector.y), (sin * vector.x) + (cos * vector.y));
    }
}