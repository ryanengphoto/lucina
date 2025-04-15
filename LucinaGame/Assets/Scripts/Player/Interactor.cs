using System.Collections;
using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement; 

interface IInteractable
{
    public void Interact();
}

public class Interactor : MonoBehaviour
{
    public GameObject openCross;
    public GameObject closedCross;
    public Transform InteractorSource;
    public float InteractRange;
    public TextMeshProUGUI momentosText;
    public AudioSource audioSource;
    public AudioSource sirenSound;
    public PlayerMovement playerMovement;

    private void Start()
    {
        UpdateMomentosText();
    }

    void Update()
    {
        Ray r = new Ray(InteractorSource.position, InteractorSource.forward);
        if (Physics.Raycast(r, out RaycastHit hitInfo, InteractRange))
        {
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
                    UpdateMomentosText();
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

    private IEnumerator PlaySirenAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay); 

        sirenSound.Play(); 
    }

    void UpdateMomentosText()
    {
        if (momentosText != null)
        {
            momentosText.text = "Mementos: " + GameManager.Instance.momentos.ToString() + "/6";
        }
    }
}
