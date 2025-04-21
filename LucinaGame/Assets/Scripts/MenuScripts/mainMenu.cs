using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;
using Kino;

public class mainMenu : MonoBehaviour
{
    public GameObject monster;
    public GameObject monster2;
    public GameObject monster3;
    public GameObject skip;
    public CanvasGroup fadeCanvasGroup;
    public TextMeshProUGUI instructionsText;
    public AudioSource typingAudioSource;
    public float fadeDuration = 1f;
    public float typingSpeed = 0.05f;
    private AnalogGlitch glitchEffect;
    public Light[] lightObjects;
    public float maxIntensity = 3f; 
    bool typing = false;
    public AudioSource staticSound;

    private void Start()
    {
        staticSound.volume = 0;
        instructionsText.text = "";
        skip.SetActive(false);

        foreach (Light light in lightObjects)
        {
            light.intensity = maxIntensity;
        }

        monster.SetActive(false);
        monster2.SetActive(false);
        monster3.SetActive(false);
        glitchEffect = Camera.main.GetComponent<AnalogGlitch>();

        glitchEffect.scanLineJitter = 0;
        glitchEffect.colorDrift = 0;

        fadeCanvasGroup.alpha = 0f;
        fadeCanvasGroup.gameObject.SetActive(false);

        StartCoroutine(RandomGlitchRoutine());
        StartCoroutine(RandomLightFlicker());
    }

    public void startGame()
    {
        if (typing) return;
        fadeCanvasGroup.gameObject.SetActive(true);
        StartCoroutine(StartGameSequence());
    }

    public void Update()
    {
        if (typing && Input.GetKeyDown(KeyCode.Space))
        {
            typing = false;
            skip.SetActive(false);
            staticSound.volume = 0;
            MusicClass musicObject = MusicClass._instance;
            StopAllCoroutines();
            musicObject.StopMusic();
            musicObject.SetPitch(1);
            fadeCanvasGroup.alpha = 1f;
            instructionsText.text = "Loading...";
            SceneManager.LoadScene("MAIN_MAP");
            return;
        }

        if (typing && Input.anyKeyDown)
        {
            skip.SetActive(true);
        }
    }

    public void quitGame()
    {
        Application.Quit();
    }

    public void goOptions()
    {
        SceneManager.LoadScene("OPTIONS_MENU");
    }

    public void goMenuFromOptions()
    {
        SceneManager.LoadScene("MAIN_MENU");
    }

    public void goMenu()
    {
        GameManager.Instance.ResetGame();
        SceneManager.LoadScene("MAIN_MENU");
    }

    private IEnumerator StartGameSequence()
    {

        typing = true;
        staticSound.volume = 0;
        yield return StartCoroutine(FadeToBlack());
        yield return StartCoroutine(TypeText(instructionsText, "Wake Up"));
        MusicClass musicObject = MusicClass._instance;
        musicObject.StopMusic();

        yield return new WaitForSeconds(1f);
        yield return StartCoroutine(TypeText(instructionsText, "You Must Atone"));

        yield return new WaitForSeconds(1f);
        yield return StartCoroutine(TypeText(instructionsText, "Find And Collect Their Mementos"));

        yield return new WaitForSeconds(1f);
        yield return StartCoroutine(TypeText(instructionsText, "And You Will Be Free."));

        yield return new WaitForSeconds(2f);
        SceneManager.LoadScene("MAIN_MAP");
    }

    private IEnumerator FadeToBlack()
    {
        float elapsedTime = 0f;
        float startPitch = 1f;
        float endPitch = 0f;

        MusicClass musicObject = MusicClass._instance;

        while (elapsedTime < fadeDuration)
        {
            float t = elapsedTime / fadeDuration;
            fadeCanvasGroup.alpha = Mathf.Lerp(0f, 1f, t);

            if (musicObject != null)
            {
                musicObject.SetPitch(Mathf.Lerp(startPitch, endPitch, t));
            }
            elapsedTime += Time.deltaTime;

            yield return null;
        }

        fadeCanvasGroup.alpha = 1f;

        if (musicObject != null)
        {
            musicObject.SetPitch(endPitch);
        }
    }

   private IEnumerator TypeText(TextMeshProUGUI textElement, string message)
    {
        textElement.text = "";
        typing = true;

        foreach (char letter in message.ToCharArray())
        {
            if(letter != ' '){
                typingAudioSource.Play(); 
            }

            textElement.text += letter;
            yield return new WaitForSeconds(typingSpeed);
        }

        typing = false;

    }

    private IEnumerator RandomGlitchRoutine()
    {
        while (true)
        {
            yield return new WaitForSeconds(Random.Range(5f, 20f));

            float glitchIntensity = Random.Range(0.2f, 0.3f);
            float duration = Random.Range(0.5f, 1f);

            int temp = Random.Range(0, 2);

            for (float t = 0; t < duration; t += Time.deltaTime)
            {
                if(t >= duration/2){
                    staticSound.volume = (t / duration)/8;
                    foreach (Light light in lightObjects)
                    {
                        light.intensity = 0;
                    }
                    if(temp == 0){
                        monster.SetActive(true);
                    } else if (temp == 1){
                        monster2.SetActive(true);
                    } else if (temp == 2){
                        monster3.SetActive(true);
                    }
                }
                float lerpValue = Mathf.Lerp(0f, glitchIntensity, t / duration);
                ApplyGlitch(lerpValue);
                yield return null;
            }

            ApplyGlitch(glitchIntensity);

            yield return new WaitForSeconds(0.2f);
            
            monster.SetActive(false);
            monster2.SetActive(false);
            monster3.SetActive(false);
            staticSound.volume = 0;

            foreach (Light light in lightObjects)
            {
                light.intensity = maxIntensity;
            }

            for (float t = 0; t < duration; t += Time.deltaTime)
            {
                float lerpValue = Mathf.Lerp(glitchIntensity, 0f, t / duration);
                if (lightObjects != null)

                ApplyGlitch(lerpValue);
                yield return null;
            }

            ApplyGlitch(0f);
        }
    }

    private void ApplyGlitch(float intensity)
    {
        MusicClass musicObject = MusicClass._instance;
        if (glitchEffect != null)
        {
            glitchEffect.scanLineJitter = intensity;
            glitchEffect.colorDrift = intensity;
        }

        if (musicObject != null)
        {
            musicObject.SetPitch(1f - intensity * 0.5f);
        }
    }

    private IEnumerator RandomLightFlicker()
    {
        while (true)
        {
            float waitTime = Random.Range(3f, 10f);
            yield return new WaitForSeconds(waitTime);

            int flickers = Random.Range(1, 4);

            for (int i = 0; i < flickers; i++)
            {
                foreach (Light light in lightObjects)
                {
                    light.intensity = 0f;
                }

                yield return new WaitForSeconds(0.05f);

                foreach (Light light in lightObjects)
                {
                    light.intensity = maxIntensity;
                }

                yield return new WaitForSeconds(0.05f);
            }
        }
    }

}
