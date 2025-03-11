using System.Collections;
using UnityEngine;

public class LightBlinker : MonoBehaviour
{
    public Light[] lightObjects;  // Array of Light components
    public float maxIntensity = 1f;  // Maximum intensity for the light
    public float fadeDuration = 1f;  // Duration for fading the light in and out
    public float minWaitTime = 1f;  // Minimum wait time between blinks
    public float maxWaitTime = 3f;  // Maximum wait time between blinks

    private void Start()
    {
        // If no lights are assigned, try to get them automatically
        if (lightObjects.Length == 0)
        {
            lightObjects = GetComponentsInChildren<Light>(); // Gets all child light components
        }
        
        // Start the coroutine to blink lights
        StartCoroutine(BlinkLights());
    }

    private IEnumerator BlinkLights()
    {
        while (true)
        {
            // Randomize the wait time before the light blinks again
            float waitTime = Random.Range(minWaitTime, maxWaitTime);
            yield return new WaitForSeconds(waitTime);

            // Fade all lights from 0 to max intensity
            yield return StartCoroutine(FadeLights(0f, maxIntensity, fadeDuration));

            // Fade all lights back to 0 intensity
            yield return StartCoroutine(FadeLights(maxIntensity, 0f, fadeDuration));
        }
    }

    // Method to fade all lights from a start intensity to an end intensity over a duration
    private IEnumerator FadeLights(float startIntensity, float endIntensity, float duration)
    {
        float elapsedTime = 0f;

        // Fade each light's intensity
        while (elapsedTime < duration)
        {
            foreach (Light light in lightObjects)
            {
                light.intensity = Mathf.Lerp(startIntensity, endIntensity, elapsedTime / duration);
            }
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        // Ensure final intensity is exactly as requested
        foreach (Light light in lightObjects)
        {
            light.intensity = endIntensity;
        }
    }
}
