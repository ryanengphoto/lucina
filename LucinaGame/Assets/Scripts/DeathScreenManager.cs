using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Video;
using TMPro;

public class DeathScreenManager : MonoBehaviour
{
    public GameObject videoPlayer;        
    public TextMeshProUGUI gameOverText;   
    public TextMeshProUGUI roomsText;     
    public Button giveUpButton;        
    public Button tryAgainButton;        
    public float typeSpeed = 0.1f;  

    private void Start()
    {
        StartCoroutine(DeathSequence());
    }

    private IEnumerator DeathSequence()
    {
        yield return new WaitForSeconds(2f);

        videoPlayer.gameObject.SetActive(false);

        string gameOverMessage = "Game Over";
        string roomsMessage = "Momentos Collected: " + GameManager.Instance.momentos + "/8";  

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
            yield return new WaitForSeconds(typeSpeed);
        }
    }
}
