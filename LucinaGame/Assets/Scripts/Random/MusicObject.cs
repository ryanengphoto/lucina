using UnityEngine;

public class MusicClass : MonoBehaviour
{
    public static MusicClass _instance;
    private AudioSource _audioSource;

    private void Awake()
    {
        if (_instance != null && _instance != this)
        {
            Destroy(gameObject);
            return;
        }

        _instance = this;
        DontDestroyOnLoad(gameObject);

        _audioSource = GetComponent<AudioSource>();
    }

    private void OnDestroy()
    {
        if (_instance == this)
        {
            _instance = null;
        }
    }

    public void PlayMusic()
    {
        if (_audioSource != null && !_audioSource.isPlaying)
        {
            _audioSource.Play();
        }
    }

    public void StopMusic()
    {
        if (_audioSource != null)
        {
            _audioSource.Stop();
        }
    }

    public void SetPitch(float pitch)
    {
        if (_audioSource != null)
        {
            _audioSource.pitch = pitch;
        }
    }
}
