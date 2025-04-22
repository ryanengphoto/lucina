using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AmbiencePlayer : MonoBehaviour
{
    public AudioClip[] ambienceSounds;
    public AudioSource audioSource;
    private int randomIndex = 0;

    void Start()
    {
        StartCoroutine(PlayAmbienceSounds());
    }

    private IEnumerator PlayAmbienceSounds()
    {
        while (true)
        {
            randomIndex = Random.Range(0, ambienceSounds.Length);
            audioSource.PlayOneShot(ambienceSounds[randomIndex], ambienceSounds[randomIndex].length);
            float delay = Random.Range(60, 120) + ambienceSounds[randomIndex].length;
            yield return new WaitForSeconds(delay);
        }
    }
}
