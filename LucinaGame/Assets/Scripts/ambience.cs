using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;


public class ambience : MonoBehaviour
{
    // Start is called before the first frame update
    public AudioSource ambience1;
    public AudioSource ambience2;
    public AudioSource ambience3;
    public AudioSource ambience4;
    System.Random rnd = new System.Random();
    List<AudioSource> sources = new List<AudioSource>();
    int currentSound;
    void Start()
    {
        sources.Add(ambience1);
        sources.Add(ambience2);
        sources.Add(ambience3);
        sources.Add(ambience4);
        int num = rnd.Next(0, 4);

        StartCoroutine(playSounds());

    }

    AudioSource selectSound() 
    {
        int next = currentSound;
        
        while (next == currentSound) {
            next = rnd.Next(0, 4);
        }

        currentSound = next;
        return sources[next];
    }

    IEnumerator playSounds()
    {
        while (true)
        {
            selectSound().Play(0);
            yield return new WaitForSecondsRealtime(rnd.Next(25,40));
        }
    }
}
