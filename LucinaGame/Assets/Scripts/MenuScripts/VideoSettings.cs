using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class VideoSettings : MonoBehaviour
{
    public TMP_Dropdown dropdown; 
    public Slider sensitivitySlider;

    void Start()
    {
        // Set current dropdown selection
        dropdown.value = Screen.fullScreenMode == FullScreenMode.FullScreenWindow ? 0 : 1;
        dropdown.onValueChanged.AddListener(ChangeScreenMode);

        sensitivitySlider.value = PlayerPrefs.GetFloat("Sensitivity", 2f);
        sensitivitySlider.onValueChanged.AddListener(SetSens);
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

    void SetSens(float sens) {
        PlayerPrefs.SetFloat("Sensitivity", sens);
    }
}

