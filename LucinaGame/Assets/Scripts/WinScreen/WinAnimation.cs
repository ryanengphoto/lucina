using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;
using Kino;
using Unity.VisualScripting;

public class WINAnimation : MonoBehaviour
{
    public GameObject menuButton;
    public float typeSpeed = 0.1f;
    public AudioSource keyPressAudio; 
    public AudioSource Music; 
    public TextMeshProUGUI instructionsText;
    bool typing = false;
    private AnalogGlitch glitchEffect;
    public CanvasGroup fadeCanvasGroup;
    
     
    private void Start()
    {
        fadeCanvasGroup.gameObject.SetActive(false);
        fadeCanvasGroup.alpha = 0f;
        if (typing) return;
        glitchEffect = Camera.main.GetComponent<AnalogGlitch>();

        glitchEffect.scanLineJitter = 0;
        glitchEffect.colorDrift = 0;

        Music.volume = 0f;

        StartCoroutine(StartWinSequence());
    }

    private IEnumerator StartWinSequence()
    {

        typing = true;
        yield return new WaitForSeconds(2f);
        yield return StartCoroutine(TypeMemento(instructionsText, "6 / 6"));
        yield return new WaitForSeconds(2f);
        Music.Play();
        StartCoroutine(FadeIn(Music, 4f, 0.4f));
        yield return new WaitForSeconds(4f);
        yield return StartCoroutine(TypeText(instructionsText, "You Did As Asked."));

        yield return new WaitForSeconds(1f);
        yield return StartCoroutine(TypeText(instructionsText, "But They're Still Watching."));

        yield return new WaitForSeconds(1f);
        yield return StartCoroutine(TypeText(instructionsText, "You Cannot Escape The Past."));

        yield return new WaitForSeconds(1f);
        yield return StartCoroutine(TypeText(instructionsText, "Only Accept It."));
        yield return new WaitForSeconds(1f);
        
        Cursor.lockState = CursorLockMode.None; 
        Cursor.visible = true;
        menuButton.SetActive(true);
    }

    private IEnumerator FadeIn(AudioSource audioSource, float duration, float maxVolume)
    {
        if (!audioSource.isPlaying) audioSource.Play();
        float time = 0f;
        audioSource.volume = 0f;

        while (time < duration)
        {
            time += Time.deltaTime;
            audioSource.volume = Mathf.Lerp(0f, maxVolume, time / duration);
            yield return null;
        }

        audioSource.volume = maxVolume;
    }

    public IEnumerator FadeOut(AudioSource audioSource, float duration)
    {
        float startVolume = audioSource.volume;

        float time = 0f;
        while (time < duration)
        {
            fadeCanvasGroup.alpha = Mathf.Lerp(0f, 1f, time / duration);
            time += Time.deltaTime;
            audioSource.volume = Mathf.Lerp(startVolume, 0f, time / duration);
            yield return null;
        }

        audioSource.volume = 0f;
        audioSource.Stop();
        GameManager.Instance.ResetGame();
        SceneManager.LoadScene("MAIN_MENU");
    }

    private IEnumerator TypeText(TextMeshProUGUI textElement, string message)
    {
        textElement.text = "";
        textElement.color = Color.white;
        foreach (char letter in message.ToCharArray())
        {
            textElement.text += letter;
            if(letter != ' '){
                keyPressAudio.Play(); 
            }
             
            yield return new WaitForSeconds(typeSpeed);
        }
    }
    
    private IEnumerator TypeMemento(TextMeshProUGUI textElement, string message)
    {
        textElement.text = "";
        textElement.color = Color.red;
        foreach (char letter in message.ToCharArray())
        {
            textElement.text += letter;
            if(letter != ' '){
                keyPressAudio.Play(); 
            }
            yield return new WaitForSeconds(0.3f);
        }
    }

    public void MenuButton()
    {
        fadeCanvasGroup.gameObject.SetActive(true);
        StartCoroutine(FadeOut(Music, 2f));
    }
}
