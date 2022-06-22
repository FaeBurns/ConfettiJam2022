using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>
/// Component responsible for handling the Return To Menu button.
/// </summary>
public class ReturnToMenuButton : MonoBehaviour
{
    /// <summary>
    /// Click handler.
    /// </summary>
    public void Click()
    {
        SceneManager.LoadScene(0);
    }
}