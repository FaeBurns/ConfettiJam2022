using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

public class RickButton : MonoBehaviour
{
    const string url = @"https://www.youtube.com/watch?v=dQw4w9WgXcQ";

    public void Click()
    {
        Application.OpenURL(url);
    }
}