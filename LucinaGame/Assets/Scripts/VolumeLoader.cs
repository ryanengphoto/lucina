
using UnityEngine;
using UnityEngine.Audio;

public class VolumeLoader : MonoBehaviour
{

    [SerializeField] private AudioMixer audioMixer;

    void Start()
    {
        if (PlayerPrefs.HasKey("Volume"))
        {
            AudioListener.volume = PlayerPrefs.GetFloat("Volume");
        }
        if (PlayerPrefs.HasKey("MusicVolume"))
        {
            float dB = Mathf.Log10(Mathf.Max(PlayerPrefs.GetFloat("MusicVolume"), 0.0001f)) * 20;
            audioMixer.SetFloat("MusicVolume", dB);
        }
        if (PlayerPrefs.HasKey("SfxVolume"))
        {
            float dB = Mathf.Log10(Mathf.Max(PlayerPrefs.GetFloat("SfxVolume"), 0.0001f)) * 20;
            audioMixer.SetFloat("SfxVolume", dB);
        }
    }

}
