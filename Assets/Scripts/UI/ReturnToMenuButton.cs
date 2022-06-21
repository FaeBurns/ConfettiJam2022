using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ReturnToMenuButton : MonoBehaviour
{
    public void Click()
    {
        SceneManager.LoadScene(0);
    }
}
