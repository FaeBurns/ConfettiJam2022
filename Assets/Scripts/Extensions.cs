using System;
using System.Collections.Generic;

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
}