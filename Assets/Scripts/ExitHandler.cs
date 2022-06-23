using UnityEngine;

public class ExitHandler : MonoBehaviour
{
#if !UNITY_EDITOR
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            Application.Quit();
        }
    }
#endif
}