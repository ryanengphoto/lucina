using UnityEngine;
using UnityEngine.UI;

public class BrightnessController : MonoBehaviour
{
    public Slider brightnessSlider;

    void Start()
    {
        
        brightnessSlider.value = PlayerPrefs.GetFloat("Brightness", 0f);
        RenderSettings.ambientLight = Color.white * brightnessSlider.value;
        brightnessSlider.onValueChanged.AddListener(SetBrightness);
    }

    void SetBrightness(float brightness) {
        RenderSettings.ambientLight = Color.white * brightness;
        PlayerPrefs.SetFloat("Brightness", brightness);
    }
}