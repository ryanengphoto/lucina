using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Kino;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class LookAtMonster : MonoBehaviour
{
    public AnalogGlitch glitchEffect;
    public float health = 100.0f;
    public bool looking, canRecharge;
    public bool lookingAngelOne;
    public bool lookingAngelTwo;
    public bool lookingAngelThree;
    public bool lookingAngelFour;
    public GameObject angelOne;
    public GameObject angelTwo;
    public GameObject angelThree;
    public GameObject angelFour;
    public AudioSource glitchSound;
    public float regenRate = 5.0f;
    public float damage = 10.0f;
    public raycastMonster detectedScript;
    public GameObject Monster;
    public Camera playerCam;
    public MonsterAI monsterAi;
    public AudioSource jumpscareSound;
    public AudioSource heartBeat;
    public GameObject jumpscareImage;
    public float jumpscareDuration = 1f;
    public bool inGarden = false;

    void Start()
    {
        health = 100f;
    }

    void Update()
    {
        Plane[] planes = GeometryUtility.CalculateFrustumPlanes(playerCam);

        if(inGarden){
            if (GeometryUtility.TestPlanesAABB(planes, angelOne.GetComponent<Renderer>().bounds)){
                lookingAngelOne = true;
                angelOne.GetComponent<AngelAI>().SetLookingAt(true);
            } else {
                lookingAngelOne = false;
                angelOne.GetComponent<AngelAI>().SetLookingAt(false);
            }
            if (GeometryUtility.TestPlanesAABB(planes, angelTwo.GetComponent<Renderer>().bounds)){
                lookingAngelTwo = true;
                angelTwo.GetComponent<AngelAI>().SetLookingAt(true);
            } else {
                lookingAngelTwo = false;
                angelTwo.GetComponent<AngelAI>().SetLookingAt(false);
            }
            if (GeometryUtility.TestPlanesAABB(planes, angelThree.GetComponent<Renderer>().bounds)){
                lookingAngelThree = true;
                angelThree.GetComponent<AngelAI>().SetLookingAt(true);
            } else {
                lookingAngelThree = false;
                angelThree.GetComponent<AngelAI>().SetLookingAt(false);
            }
            if (GeometryUtility.TestPlanesAABB(planes, angelFour.GetComponent<Renderer>().bounds)){
                lookingAngelFour = true;
                angelFour.GetComponent<AngelAI>().SetLookingAt(true);
            } else {
                lookingAngelFour = false;
                angelFour.GetComponent<AngelAI>().SetLookingAt(false);
            }
        } else {
            lookingAngelOne = true;
            angelOne.GetComponent<AngelAI>().SetLookingAt(true);
            lookingAngelTwo = true;
            angelTwo.GetComponent<AngelAI>().SetLookingAt(true);
            lookingAngelThree = true;
            angelThree.GetComponent<AngelAI>().SetLookingAt(true);
            lookingAngelFour = true;
            angelFour.GetComponent<AngelAI>().SetLookingAt(true);
        }

        if (GeometryUtility.TestPlanesAABB(planes, Monster.GetComponent<Renderer>().bounds))
        {
            looking = true;
        }
        else
        {
            looking = false;
        }
        
        if (detectedScript.detected && looking && Vector3.Distance(transform.position, Monster.transform.position) <= 35.0f)
        {
            glitchEffect.scanLineJitter += Time.deltaTime / 1.5f;
            glitchEffect.colorDrift += Time.deltaTime / 1.5f;
            glitchSound.volume += Time.deltaTime;
            heartBeat.volume += Time.deltaTime;
            health -= damage * Time.deltaTime;
        }
        else if (detectedScript.detected && looking && Vector3.Distance(transform.position, Monster.transform.position) <= 50.0f)
        {
            glitchEffect.scanLineJitter += Time.deltaTime / 5f;
            glitchEffect.colorDrift += Time.deltaTime / 5f;
            glitchSound.volume += Time.deltaTime / 25;
            heartBeat.volume += Time.deltaTime / 25;
            health -= damage * Time.deltaTime / 25;
        }
        else if (detectedScript.detected && looking && Vector3.Distance(transform.position, Monster.transform.position) <= 100.0f)
        {
            if (glitchEffect.scanLineJitter >= 0.2f)
            {
                glitchEffect.scanLineJitter = 0.2f;
            }
            if (glitchEffect.colorDrift >= 0.2f)
            {
                glitchEffect.colorDrift = 0.2f;
            }
            if (heartBeat.volume >= 0.2f)
            {
                heartBeat.volume = 0.2f;
            }
            glitchEffect.scanLineJitter += Time.deltaTime / 50f;
            glitchEffect.colorDrift += Time.deltaTime / 50f;
            heartBeat.volume += Time.deltaTime / 50;
        }
        else
        {
            if (glitchEffect.scanLineJitter > 0)
            {
                glitchEffect.scanLineJitter -= Time.deltaTime / 1.5f;
            }
            if (glitchEffect.colorDrift > 0)
            {
                glitchEffect.colorDrift -= Time.deltaTime / 1.5f;
            }
            if (glitchSound.volume > 0)
            {
                if (glitchSound.volume <= 0.01)
                {
                    glitchSound.volume -= Time.deltaTime / 50;
                }
                else if (glitchSound.volume <= 0.05)
                {
                    glitchSound.volume -= Time.deltaTime / 10;
                }
                else
                {
                    glitchSound.volume -= Time.deltaTime / 2.5f;
                }
            }
            if (heartBeat.volume > 0)
            {
                heartBeat.volume -= Time.deltaTime / 10;
            }

            if (health < 100)
            {
                health += regenRate * Time.deltaTime;
            }
            else
            {
                health = 100;
            }
        }

        if (health <= 0)
        {
            glitchEffect.scanLineJitter = 2;
            glitchEffect.colorDrift = 2;
            glitchSound.volume = 1.5f;
            StartCoroutine(ShowJumpScare());
        }
    }

    IEnumerator ShowJumpScare()
    {
        if (!jumpscareSound.isPlaying)
        {
            jumpscareSound.Play();
        }

        jumpscareImage.SetActive(true);

        float flickerDuration = 2f;
        float flickerTime = 0f;

        while (flickerTime < flickerDuration)
        {
            jumpscareImage.SetActive(!jumpscareImage.activeSelf);
            flickerTime += 0.1f;
            yield return new WaitForSeconds(0.1f);  
        }

        jumpscareImage.SetActive(true);
        
        SceneManager.LoadScene("DEATH_SCENE");
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("garden") && inGarden == false)
        {
            inGarden = true;
        }
        if (other.CompareTag("monster"))
        {
            glitchEffect.scanLineJitter = 2;
            glitchEffect.colorDrift = 2;
            glitchSound.volume = 1.5f;
            StartCoroutine(ShowJumpScare());
        }
        if (other.CompareTag("safeRoom") && inGarden == true)
        {
            lookingAngelOne = true;
            angelOne.GetComponent<AngelAI>().SetLookingAt(true);
            lookingAngelTwo = true;
            angelTwo.GetComponent<AngelAI>().SetLookingAt(true);
            lookingAngelThree = true;
            angelThree.GetComponent<AngelAI>().SetLookingAt(true);
            lookingAngelFour = true;
            angelFour.GetComponent<AngelAI>().SetLookingAt(true);
            inGarden = false;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("garden") && inGarden == true)
        {
            inGarden = false;
        }
        if (other.CompareTag("safeRoom") && inGarden == false)
        {
            
            inGarden = true;
        }
    }
}
