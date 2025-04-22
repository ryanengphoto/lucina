using System.Collections;
using UnityEngine;
using UnityEngine.AI;

public class LightBlinker : MonoBehaviour
{
    public Light[] lightObjects;
    private float maxIntensity = 2f;
    private float minWaitTime = 2f;
    private float maxWaitTime = 5f;
    int temp;
    
    public bool favorOff = false;
    public bool favorOn = false;
    public bool off = false;

    private void Start()
    {
        if (lightObjects.Length == 0)
        {
            lightObjects = GetComponentsInChildren<Light>();
        }

        foreach (Light light in lightObjects)
        {
            light.intensity = 0f;
        }

        StartCoroutine(BlinkLights());
    }

    private IEnumerator BlinkLights()
    {
        while (true)
        {
            if(off){
                temp = 3;
            }
            else if(favorOff){
                temp = 2;
            } else if (favorOn){
                temp = 1;
            } else {
                temp = Random.Range(1, 3);
            }
            
            float waitTime = Random.Range(minWaitTime, maxWaitTime);
            yield return new WaitForSeconds(waitTime);

            int flickers = Random.Range(1, 4);

            for (int i = 0; i < flickers; i++)
            {
                foreach (Light light in lightObjects)
                {
                    if(temp == 2){
                        light.intensity = maxIntensity;
                    } else if(temp == 1){
                        light.intensity = 0;
                    }
                }

                yield return new WaitForSeconds(0.05f);

                foreach (Light light in lightObjects)
                {
                    if(temp == 2){
                        light.intensity = 0;
                    } else if(temp == 1){
                        light.intensity = maxIntensity;
                    }
                    
                }

                yield return new WaitForSeconds(0.05f);
            }
        }
    }
}
