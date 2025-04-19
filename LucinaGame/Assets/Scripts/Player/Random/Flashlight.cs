using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Flashlight : MonoBehaviour
{
    public GameObject flashlight;
    public AudioSource turnOn;
    public AudioSource turnOff;
    
    private bool on;
    private bool off;

    public float cooldownDuration = 0.5f;

    private float currentCooldown = 0f;
    void Start()
    {
        off = true;
        flashlight.SetActive(false);
    }

    void Update()
    {
        if (currentCooldown > 0)
        {
            currentCooldown -= Time.deltaTime;
        }

        if (off && Input.GetKeyDown(KeyCode.F) && currentCooldown <= 0)
        {
            flashlight.SetActive(true);
            turnOn.Play();
            off = false;
            on = true;
            currentCooldown = cooldownDuration;
        }
        else if (on && Input.GetKeyDown(KeyCode.F) && currentCooldown <= 0)
        {
            flashlight.SetActive(false);
            turnOff.Play();
            off = true;
            on = false;
            currentCooldown = cooldownDuration;
        }
    }
}
