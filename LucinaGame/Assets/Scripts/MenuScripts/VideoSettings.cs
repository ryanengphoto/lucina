using UnityEngine;
using TMPro;

public class VideoSettings : MonoBehaviour
{
    public TMP_Dropdown dropdown; 

    void Start()
    {
        // Set current dropdown selection
        dropdown.value = Screen.fullScreenMode == FullScreenMode.FullScreenWindow ? 0 : 1;
        dropdown.onValueChanged.AddListener(ChangeScreenMode);
    }

    void ChangeScreenMode(int option)
    {
        if (option == 0)
        {
            Screen.fullScreenMode = FullScreenMode.FullScreenWindow; // Fullscreen
        }
        else if (option == 1)
        {
            Screen.fullScreenMode = FullScreenMode.Windowed; // Windowed
        }

    }
}
