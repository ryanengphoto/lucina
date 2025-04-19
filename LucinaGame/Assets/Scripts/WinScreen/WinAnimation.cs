using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;
using Kino;

public class WINAnimation : MonoBehaviour
{
    public GameObject menuButton;
    public float typeSpeed = 0.1f;
    public AudioSource keyPressAudio; 
    public AudioSource Music; 
    public TextMeshProUGUI instructionsText;
    bool typing = false;
    private AnalogGlitch glitchEffect;
    
     
    private void Start()
    {
        if (typing) return;
        glitchEffect = Camera.main.GetComponent<AnalogGlitch>();

        glitchEffect.scanLineJitter = 0;
        glitchEffect.colorDrift = 0;

        Cursor.lockState = CursorLockMode.None; 
        Cursor.visible = true;
        Music.volume = 0f;

        StartCoroutine(FadeIn(Music, 2f, 0.4f));
        StartCoroutine(StartWinSequence());
    }

    private IEnumerator StartWinSequence()
    {

        typing = true;
        yield return new WaitForSeconds(1f);
        yield return StartCoroutine(TypeText(instructionsText, "You Did As Asked."));

        yield return new WaitForSeconds(1f);
        yield return StartCoroutine(TypeText(instructionsText, "But They're Still Watching."));

        yield return new WaitForSeconds(1f);
        yield return StartCoroutine(TypeText(instructionsText, "You Cannot Escape The Past."));

        yield return new WaitForSeconds(1f);
        yield return StartCoroutine(TypeText(instructionsText, "Only Acceptance."));
        yield return new WaitForSeconds(1f);
        
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

    private IEnumerator TypeText(TextMeshProUGUI textElement, string message)
    {
        textElement.text = "";
        foreach (char letter in message.ToCharArray())
        {
            textElement.text += letter;
            keyPressAudio.Play();  
            yield return new WaitForSeconds(typeSpeed);
        }
    }

    public void MenuButton()
    {
        GameManager.Instance.ResetGame();
        SceneManager.LoadScene("MAIN_MENU");
    }
}
