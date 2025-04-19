using UnityEngine;
using UnityEngine.UI;

public class InitBrightness : MonoBehaviour
{
    public Slider brightnessSlider;
    void Start()
    {
        
        brightnessSlider.value = PlayerPrefs.GetFloat("Brightness", 0f);
        brightnessSlider.onValueChanged.AddListener(SetBrightness);
    }

    void SetBrightness(float brightness) {
        PlayerPrefs.SetFloat("Brightness", brightness);
    }
}
