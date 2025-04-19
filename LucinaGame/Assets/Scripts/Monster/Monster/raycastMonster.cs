using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class raycastMonster : MonoBehaviour
{
    public GameObject playerObj;
    public Transform monsterTransform;
    public bool detected;
    public Vector3 offset;
    void Update()
    {
        Vector3 direction = (playerObj.transform.position - monsterTransform.position).normalized;

        RaycastHit hit;

        if (Physics.Raycast(monsterTransform.position + offset, direction, out hit, Mathf.Infinity))
        {
            if (hit.collider.gameObject == playerObj)
            {
                detected = true;
            }
            else
            {
                detected = false;
            }
        }
        else
        {
            detected = false;
        }
    }
}
