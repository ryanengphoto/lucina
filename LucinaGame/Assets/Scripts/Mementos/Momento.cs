using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Light))]
public class Momento : MonoBehaviour, IInteractable
{
    public float floatStrength = 0.5f;
    public float floatSpeed = 2f;
    public float rotationSpeed = 50f; 

    private Vector3 initialPosition;
    private Light glowLight;
    private float lightBaseIntensity;
    private float lightPulseSpeed = 5f; 
    void Start()
    {
        initialPosition = transform.position;
        glowLight = GetComponent<Light>();
        lightBaseIntensity = glowLight.intensity;
    }

    void Update()
    {
        float newY = Mathf.Sin(Time.time * floatSpeed) * floatStrength;
        transform.position = initialPosition + new Vector3(0, newY, 0);

        transform.Rotate(Vector3.up, rotationSpeed * Time.deltaTime);

        glowLight.intensity = lightBaseIntensity + Mathf.Sin(Time.time * lightPulseSpeed) * 0.2f;
    }

    public void Interact()
    {
        Destroy(gameObject);
    }
}
