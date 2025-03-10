using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

public class mainMenu : MonoBehaviour
{
    public GameObject blackScreen;              // Black screen panel
    public TextMeshProUGUI messageText;         // Text for messages
    public AudioSource typingSound;             // AudioSource for typing sound
    public float typeSpeed = 0.1f;              // Speed of typing effect
    public AudioSource music;                   // Background music to stop
    private bool skipTyping = false;            // Flag to skip typing

    public void startGame()
    {
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
        // Enable the black screen and fade it in
        blackScreen.SetActive(true);
        music.Stop();  // Stop background music
        blackScreen.GetComponent<CanvasGroup>().alpha = 0f;
        float fadeDuration = 1f;
        float elapsedTime = 0f;

        while (elapsedTime < fadeDuration)
        {
            blackScreen.GetComponent<CanvasGroup>().alpha = Mathf.Lerp(0f, 1f, elapsedTime / fadeDuration);
            elapsedTime += Time.deltaTime;
            yield return null;
        }
        blackScreen.GetComponent<CanvasGroup>().alpha = 1f;

        // Start typing the messages one by one
        yield return StartCoroutine(TypeText(messageText, "Press F to Activate Flashlight"));
        yield return new WaitForSeconds(1);
        yield return StartCoroutine(TypeText(messageText, "Hold Shift to Run"));
        yield return new WaitForSeconds(1);
        yield return StartCoroutine(TypeText(messageText, "And Don't Stare At The Monster."));
        yield return new WaitForSeconds(1);

        // Load the MAIN_MAP scene after typing finishes or if skip is pressed
        SceneManager.LoadScene("MAIN_MAP");
    }

    private IEnumerator TypeText(TextMeshProUGUI textElement, string message)
    {
        textElement.text = ""; // Clear the previous message
        skipTyping = false; // Reset skip flag

        foreach (char letter in message.ToCharArray())
        {
            textElement.text += letter;

            // Play typing sound from AudioSource
            if (typingSound != null)
            {
                typingSound.Play(); // Play the typing sound each time a character is typed
            }

            // Check for spacebar press or any other key
            if (Input.GetKeyDown(KeyCode.Space)) // Spacebar skip input
            {
                skipTyping = true; // Set the flag to true if spacebar is pressed
                break; // Skip the remaining typing and proceed
            }

            // If skipTyping flag is set, skip typing early
            if (skipTyping)
            {
                textElement.text = message; // Immediately finish the message
                break;
            }

            yield return new WaitForSeconds(typeSpeed);
        }
    }
}
