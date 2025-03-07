using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

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
                    GameManager.Instance.momentos++;
                    UpdateMomentosText();
                    interactObj.Interact();
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

    void UpdateMomentosText()
    {
        if (momentosText != null)
        {
            momentosText.text = "Momentos: " + GameManager.Instance.momentos.ToString();
        }
    }
}
