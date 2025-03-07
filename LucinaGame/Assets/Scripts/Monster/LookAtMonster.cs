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
    public AudioSource glitchSound;
    public float regenRate = 5.0f;
    public float damage = 10.0f;
    public raycastMonster detectedScript;
    public GameObject Monster;
    public Camera playerCam;
    public MonsterAI monsterAi;
    public AudioSource jumpscareSound;
    public GameObject jumpscareImage;
    public float jumpscareDuration = 1f; 
    private float looked = 0;

    void Start()
    {
        health = 100f;
    }

    void Update()
    {
        Plane[] planes = GeometryUtility.CalculateFrustumPlanes(playerCam);
        if (GeometryUtility.TestPlanesAABB(planes, Monster.GetComponent<Renderer>().bounds)) {
            if(looked < 0.2){
                looked += Time.deltaTime;
            }
            if(looked >= 0.2){
                looking = true;
            }
        } else {
            if(looked > 0){
                looked -= Time.deltaTime;
            }
            if(looked <= 0){
                looking = false;
            }
        }

        if (detectedScript.detected && looking ) {
            glitchEffect.scanLineJitter += Time.deltaTime / 1.5f;
            glitchEffect.colorDrift += Time.deltaTime / 1.5f;
            glitchSound.volume += Time.deltaTime;
            health -= damage * Time.deltaTime;
            Debug.Log(health);
        } else {
            if (glitchEffect.scanLineJitter > 0) {
                glitchEffect.scanLineJitter -= Time.deltaTime/ 1.5f;
            }
            if (glitchEffect.colorDrift > 0) {
                glitchEffect.colorDrift -= Time.deltaTime/ 1.5f;
            }
            if (glitchSound.volume > 0) {
                if(glitchSound.volume <= 0.01){
                    glitchSound.volume -= Time.deltaTime / 50;
                } else if(glitchSound.volume <= 0.05){
                    glitchSound.volume -= Time.deltaTime / 10;
                } else {
                    glitchSound.volume -= Time.deltaTime / 2.5f;
                }
            }

            if (health < 100) {
                health += regenRate * Time.deltaTime;
            } else {
                health = 100;
            }
        }

        if (health <= 0) {
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
        yield return new WaitForSeconds(jumpscareDuration);
        
        jumpscareImage.SetActive(false);
        
        SceneManager.LoadScene("DeathScene");
    }
}
