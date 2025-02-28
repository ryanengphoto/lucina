using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Momento : MonoBehaviour, IInteractable
{
    public void Interact(){
        Debug.Log("clciked");
        Destroy(gameObject);
    }
}
