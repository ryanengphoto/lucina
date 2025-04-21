using System.Collections;
using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using Kino;

interface IInteractable
{
    public void Interact();
}

interface NoteInteract
{
}

public class Interactor : MonoBehaviour
{
    public AnalogGlitch glitchEffect;
    public AudioSource glitchSound;
    public GameObject openCross;
    public GameObject closedCross;
    public Transform InteractorSource;
    public float InteractRange;
    public TextMeshProUGUI momentosText;
    public AudioSource audioSource;
    public AudioSource paperNoise;
    public AudioSource sirenSound;
    public PlayerMovement playerMovement;
    public GameObject closeButton;
    public GameObject paper1;
    public GameObject paper2;
    public GameObject paper3;
    public GameObject paper4;
    public GameObject paper5;
    public GameObject CrouchInfo;
    public GameObject sprintInfo;
    public GameObject flashlightInfo;
    public GameObject interact;
    public bool inNote = false;
    private Coroutine infoCoroutine;
    public float typingSpeed = 0.05f;
    public AudioSource typingAudioSource;
    public TextMeshProUGUI mementoText;
    public CanvasGroup fadeCanvasGroup;
    public float fadeDuration = 4f;
    public GameObject jumpscareImage;
    public bool showInteract = true;


    private void Start()
    {
        flashlightInfo.SetActive(true);
        fadeCanvasGroup.alpha = 0f;
        fadeCanvasGroup.gameObject.SetActive(false);
        StartInfoCoroutine(15f);
        UpdateMomentosText();
    }

    void Update()
    {
        if(Input.GetKeyDown(KeyCode.C) || Input.GetKeyDown(KeyCode.LeftControl)){
            CrouchInfo.SetActive(false);
        }
        if(Input.GetKeyDown(KeyCode.LeftShift)){
            sprintInfo.SetActive(false);
        }
        if(Input.GetKeyDown(KeyCode.F)){
            flashlightInfo.SetActive(false);
        }

        Ray r = new Ray(InteractorSource.position, InteractorSource.forward);

        if (Physics.Raycast(r, out RaycastHit hitInfo, InteractRange))
        {
            if (hitInfo.collider.gameObject.TryGetComponent(out IInteractable interactObj))
            {
                if (!openCross.activeSelf)
                {
                    if(showInteract){
                        interact.SetActive(true);
                    }
                    
                    openCross.SetActive(true);
                    closedCross.SetActive(false);
                }

                if (Input.GetKeyDown(KeyCode.E))
                {
                    showInteract = false;
                    GameManager.Instance.UpdateMomentos(1);
                    playerMovement.makingNoise = true;
                    if(GameManager.Instance.momentos != 6){
                        StartCoroutine(UpdateMomentosText());
                    }

                    interactObj.Interact();

                    if (audioSource != null)
                    {
                        audioSource.Play();
                    }

                    if (GameManager.Instance.momentos == 1)
                    {
                        StartCoroutine(PlaySirenAfterDelay(5f));  
                    }
                    else if (GameManager.Instance.momentos == 6)
                    {
                        fadeCanvasGroup.gameObject.SetActive(true);
                        StartCoroutine(fadeToBlack());
                    }
                }
            } 
            else if (hitInfo.collider.gameObject.TryGetComponent(out NoteInteract note))
            {
                if (!openCross.activeSelf)
                {
                    if(showInteract){
                        interact.SetActive(true);
                    }
                    openCross.SetActive(true);
                    closedCross.SetActive(false);
                }

                if (Input.GetKeyDown(KeyCode.E))
                {
                    showInteract = false;
                    inNote = true;
                    interact.SetActive(false);
                    if (paperNoise != null)
                    {
                        paperNoise.Play();
                    }

                    string tag = hitInfo.collider.tag;
                    closeButton.SetActive(true);

                    if(tag == "Note1"){
                        paper1.SetActive(true);
                        CrouchInfo.SetActive(true);
                    } else if(tag == "Note2"){
                        paper2.SetActive(true);
                        CrouchInfo.SetActive(true);
                    } else if(tag == "Note3"){
                        paper3.SetActive(true);
                    } else if(tag == "Note4"){
                        paper4.SetActive(true);
                    } else {
                        paper5.SetActive(true);
                        sprintInfo.SetActive(true);
                    }

                    StartInfoCoroutine(15f);

                    Cursor.lockState = CursorLockMode.None;
                    Cursor.visible = true;
                }
            } else {
                interact.SetActive(false);
                openCross.SetActive(false);
                closedCross.SetActive(true);
            }
        }
        else
        {
            if (openCross.activeSelf)
            {
                interact.SetActive(false);
                openCross.SetActive(false);
                closedCross.SetActive(true);
            }
        }
    }

    private IEnumerator fadeToBlack()
    {
        float elapsedTime = 0f;

        while (elapsedTime < fadeDuration)
        {
            float t = elapsedTime / fadeDuration;
            fadeCanvasGroup.alpha = Mathf.Lerp(0f, 1f, t);

            elapsedTime += Time.deltaTime;

            yield return null;
        }

        closedCross.SetActive(false);
        fadeCanvasGroup.alpha = 0f;
        glitchEffect.scanLineJitter = 2;
        glitchEffect.colorDrift = 2;
        glitchSound.volume = 1f;
        
        yield return new WaitForSeconds(0.5f);
        jumpscareImage.SetActive(true);
        yield return new WaitForSeconds(0.1f);
        fadeCanvasGroup.alpha = 1f;
        jumpscareImage.SetActive(false);
        SceneManager.LoadScene("WIN_SCENE");
    }

    private IEnumerator UpdateMomentosText()
    {
        yield return new WaitForSeconds(1f);

        int currentMementos = GameManager.Instance.momentos;
        string message = currentMementos + " / 6";

        mementoText.color = GetMementoColor(currentMementos);
        yield return StartCoroutine(TypeText(mementoText, message));

        yield return new WaitForSeconds(5f);
        mementoText.text = "";
    }


    private IEnumerator TypeText(TextMeshProUGUI textElement, string message)
    {
        textElement.text = "";

        foreach (char letter in message.ToCharArray())
        {
            if(letter != ' '){
                typingAudioSource.Play(); 
            }

            textElement.text += letter;
            yield return new WaitForSeconds(typingSpeed);
        }
    }

    private void StartInfoCoroutine(float delay)
    {
        if (infoCoroutine != null)
        {
            StopCoroutine(infoCoroutine);
        }
        infoCoroutine = StartCoroutine(removeInfo(delay));
    }

    private Color GetMementoColor(int momentos)
    {
        float t = (momentos - 1) / 5f; 
        return Color.Lerp(Color.white, Color.red, t);
    }



    public void closeNote(){
        Cursor.lockState = CursorLockMode.Locked;
        StartInfoCoroutine(15f);
        Cursor.visible = false;
        paper1.SetActive(false);
        paper2.SetActive(false);
        paper3.SetActive(false);
        paper4.SetActive(false);
        paper5.SetActive(false);
        closeButton.SetActive(false);
        inNote = false;
    }

    private IEnumerator removeInfo(float delay)
    {
        yield return new WaitForSeconds(delay); 
        CrouchInfo.SetActive(false);
        sprintInfo.SetActive(false);
        flashlightInfo.SetActive(false);
    }

    private IEnumerator PlaySirenAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay); 

        sirenSound.Play(); 
    }
}
