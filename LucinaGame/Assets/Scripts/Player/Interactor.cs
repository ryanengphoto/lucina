using System.Collections;
using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

interface IInteractable
{
    public void Interact();
}

interface NoteInteract
{
}

public class Interactor : MonoBehaviour
{
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
    public bool inNote = false;
    private Coroutine infoCoroutine;
    public float typingSpeed = 0.05f;
    public AudioSource typingAudioSource;
    public TextMeshProUGUI mementoText;


    private void Start()
    {
        flashlightInfo.SetActive(true);
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
            Debug.DrawRay(InteractorSource.position, InteractorSource.forward * InteractRange, Color.red);

            if (hitInfo.collider.gameObject.TryGetComponent(out IInteractable interactObj))
            {
                if (!openCross.activeSelf)
                {
                    openCross.SetActive(true);
                    closedCross.SetActive(false);
                }

                if (Input.GetKeyDown(KeyCode.E))
                {
                    GameManager.Instance.UpdateMomentos(1);
                    playerMovement.makingNoise = true;

                    StartCoroutine(UpdateMomentosText());

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
                        SceneManager.LoadScene("WIN_SCENE");
                    }
                }
            }

            if (hitInfo.collider.gameObject.TryGetComponent(out NoteInteract note))
            {
                if (!openCross.activeSelf)
                {
                    openCross.SetActive(true);
                    closedCross.SetActive(false);
                }

                if (Input.GetKeyDown(KeyCode.E))
                {
                    inNote = true;
                    if (paperNoise != null)
                    {
                        paperNoise.Play();
                    }

                    string tag = hitInfo.collider.tag;
                    Debug.Log(tag);
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

            }
        }
        else
        {
            if (openCross.activeSelf)
            {
                openCross.SetActive(false);
                closedCross.SetActive(true);
            }
        }
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
            typingAudioSource.Play();

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
