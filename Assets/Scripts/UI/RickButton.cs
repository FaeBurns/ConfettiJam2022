using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

/// <summary>
/// Component responsible for handling the funny button.
/// </summary>
public class RickButton : MonoBehaviour
{
    /// <summary>
    /// Target URL.
    /// </summary>
    private const string Url = @"https://www.youtube.com/watch?v=iik25wqIuFo";

    /// <summary>
    /// Click handler.
    /// </summary>
    public void Click()
    {
        Application.OpenURL(Url);
    }
}