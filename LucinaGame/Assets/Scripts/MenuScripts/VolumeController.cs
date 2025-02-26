using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Audio;

public class VolumeController : MonoBehaviour
{
    [SerializeField] private AudioMixer audioMixer;
    [SerializeField] private Slider volumeSlider;
    [SerializeField] private Slider musicSlider;
    [SerializeField] private Slider sfxSlider;

    void Start()
    {
        // Load saved volume or set default
        volumeSlider.value = PlayerPrefs.GetFloat("Volume", 1f);
        AudioListener.volume = volumeSlider.value;
        volumeSlider.onValueChanged.AddListener(SetVolume);

        // Load saved volume or set default
        musicSlider.value = PlayerPrefs.GetFloat("MusicVolume", 1f);
        SetMusic(musicSlider.value);
        musicSlider.onValueChanged.AddListener(SetMusic);

        // Load saved volume or set default
        sfxSlider.value = PlayerPrefs.GetFloat("SfxVolume", 1f);
        SetSfx(sfxSlider.value);
        sfxSlider.onValueChanged.AddListener(SetSfx);
    }



    public void SetVolume(float volume)
    {
        AudioListener.volume = volume;
        PlayerPrefs.SetFloat("Volume", volume);
    }

    public void SetMusic(float volume)
    {
        // Convert linear value (0-1) to logarithmic scale (-80dB to 0dB)
        float dB = Mathf.Log10(Mathf.Max(volume, 0.0001f)) * 20;
        audioMixer.SetFloat("MusicVolume", dB);

        // Save the volume setting
        PlayerPrefs.SetFloat("MusicVolume", volume);
    }

    public void SetSfx(float volume)
    {
        // Convert linear value (0-1) to logarithmic scale (-80dB to 0dB)
        float dB = Mathf.Log10(Mathf.Max(volume, 0.0001f)) * 20;
        audioMixer.SetFloat("SfxVolume", dB);

        // Save the volume setting
        PlayerPrefs.SetFloat("SfxVolume", volume);
    }

}