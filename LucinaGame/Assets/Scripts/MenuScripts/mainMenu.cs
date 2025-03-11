using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

public class mainMenu : MonoBehaviour
{
    public CanvasGroup fadeCanvasGroup; // CanvasGroup for the fade effect
    public TextMeshProUGUI instructionsText;
    public AudioSource typingAudioSource; // Reference to the AudioSource for typing sound
    public float fadeDuration = 1f;
    public float typingSpeed = 0.05f;
    public AudioSource music;

    private void Start()
    {
        // Initially, make sure the fade canvas is inactive (transparent)
        fadeCanvasGroup.alpha = 0f; // Set to 0 so it's invisible
        fadeCanvasGroup.gameObject.SetActive(false); // Disable it to not interfere before the game starts
    }

    public void startGame()
    {
        // Activate the fade canvas group and start the fade process when the start button is clicked
        fadeCanvasGroup.gameObject.SetActive(true);
        music.Stop();
        StartCoroutine(StartGameSequence());
    }

    public void quitGame()
    {
        Application.Quit();
    }

    public void goOptions()
    {
        SceneManager.LoadScene("OPTIONS_MENU");
    }

    public void goMenu()
    {
        SceneManager.LoadScene("MAIN_MENU");
    }

    private IEnumerator StartGameSequence()
    {
        yield return StartCoroutine(FadeToBlack());

        yield return StartCoroutine(TypeText(instructionsText, "Press F to turn on flashlight"));
        yield return new WaitForSeconds(1f);
        yield return StartCoroutine(TypeText(instructionsText, "Press E to interact"));
        yield return new WaitForSeconds(1f);
        yield return StartCoroutine(TypeText(instructionsText, "Press Shift to sprint"));
        yield return new WaitForSeconds(1f);
        yield return StartCoroutine(TypeText(instructionsText, "And don't stare at the monster."));

        yield return new WaitForSeconds(2f);

        SceneManager.LoadScene("MAIN_MAP");
    }

    private IEnumerator FadeToBlack()
    {
        float elapsedTime = 0f;
        while (elapsedTime < fadeDuration)
        {
            float alpha = Mathf.Lerp(0f, 1f, elapsedTime / fadeDuration);
            fadeCanvasGroup.alpha = alpha;
            elapsedTime += Time.deltaTime;
            yield return null;
        }
        fadeCanvasGroup.alpha = 1f; // Ensure it's fully black after the fade
    }

    private IEnumerator TypeText(TextMeshProUGUI textElement, string message)
    {
        textElement.text = "";
        foreach (char letter in message.ToCharArray())
        {
            textElement.text += letter;

            // Play typing sound effect each time a letter is typed
            if (!typingAudioSource.isPlaying)
            {
                typingAudioSource.Play();
            }

            yield return new WaitForSeconds(typingSpeed);
        }
    }
}
