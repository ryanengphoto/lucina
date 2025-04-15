using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Light))]
public class Momento : MonoBehaviour, IInteractable
{
    public float floatStrength = 0.5f;      // How high it floats
    public float floatSpeed = 2f;           // How fast it floats
    public float rotationSpeed = 50f;       // Degrees per second

    private Vector3 initialPosition;
    private Light glowLight;
    private float lightBaseIntensity;
    private float lightPulseSpeed = 5f;     // Speed of light pulsing

    void Start()
    {
        initialPosition = transform.position;
        glowLight = GetComponent<Light>();
        lightBaseIntensity = glowLight.intensity;
    }

    void Update()
    {
        // Floating effect
        float newY = Mathf.Sin(Time.time * floatSpeed) * floatStrength;
        transform.position = initialPosition + new Vector3(0, newY, 0);

        // Rotation effect
        transform.Rotate(Vector3.up, rotationSpeed * Time.deltaTime);

        // Light pulsing effect
        glowLight.intensity = lightBaseIntensity + Mathf.Sin(Time.time * lightPulseSpeed) * 0.2f;
    }

    public void Interact()
    {
        Debug.Log("clicked");
        Destroy(gameObject);
    }
}
