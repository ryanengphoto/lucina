using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;

public class DeathAnimation : MonoBehaviour
{
    public GameObject videoPlayer;
    public TextMeshProUGUI gameOverText;
    public TextMeshProUGUI roomsText;
    public Button giveUpButton;
    public Button tryAgainButton;
    public float typeSpeed = 0.1f;
    public AudioSource keyPressAudio;  
    private void Start()
    {
        Cursor.lockState = CursorLockMode.None; 
        Cursor.visible = true;  
                    
        StartCoroutine(DeathSequence());

        tryAgainButton.onClick.AddListener(TryAgain);
        giveUpButton.onClick.AddListener(QuitGame);
    }

    private IEnumerator DeathSequence()
    {
        yield return new WaitForSeconds(2f);

        videoPlayer.gameObject.SetActive(false);

        string gameOverMessage = "Game Over";
        string roomsMessage = "Momentos Collected: " + GameManager.Instance.momentos + "/6";

        yield return StartCoroutine(TypeText(gameOverText, gameOverMessage));

        yield return StartCoroutine(TypeText(roomsText, roomsMessage));

        yield return new WaitForSeconds(0.5f);
        giveUpButton.gameObject.SetActive(true);
        tryAgainButton.gameObject.SetActive(true);
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

    private void TryAgain()
    {
        keyPressAudio.Play();  
        SceneManager.LoadScene("MAIN_MAP");
    }

    private void QuitGame()
    {
        keyPressAudio.Play();  
        SceneManager.LoadScene("MAIN_MENU");
    }
}
