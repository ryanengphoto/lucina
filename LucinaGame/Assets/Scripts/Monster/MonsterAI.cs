using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class MonsterAI : MonoBehaviour
{
    public Transform[] teleportSpots;
    public float spookedRadius = 15f;
    public AudioSource spookedSound;
    public GameObject player;
    private Vector3 targetPosition;
    private bool isPlayerLooking = false;
    private bool hasSpooked = false;
    private NavMeshAgent navMeshAgent;
    private bool seen = false;
    public LookAtMonster lookatmonsterScript;
    private Transform prevSpot;
    private Transform randomSpot;

    private void Start()
    {
        navMeshAgent = GetComponent<NavMeshAgent>();
        Teleport();
    }

    private void Update()
    {
        if (seen && !isPlayerLooking)
        {
            Teleport();
        }

        CheckIfPlayerIsLooking();

        if (!isPlayerLooking)
        {
            MoveTowardPlayer();
        }
    }

    private void CheckIfPlayerIsLooking()
    {
        if (lookatmonsterScript.looking)
        {
            isPlayerLooking = true;
            seen = true;

            if (isPlayerLooking && !hasSpooked && Vector3.Distance(player.transform.position, transform.position) <= spookedRadius)
            {
                PlaySpookedSound();
            }
        }
        else
        {
            isPlayerLooking = false;
        }
    }

    private void MoveTowardPlayer()
    {
        navMeshAgent.SetDestination(player.transform.position);
    }

    private void Teleport()
    {
        Transform randomSpot;
        
        do
        {
            randomSpot = teleportSpots[Random.Range(0, teleportSpots.Length)];
        }
        while (randomSpot == prevSpot);

        targetPosition = randomSpot.position;
        prevSpot = randomSpot;
        transform.position = targetPosition;
        hasSpooked = false;
        seen = false;
        Debug.Log("teleportinh");
    }

    private void PlaySpookedSound()
    {
        spookedSound.Play();
        hasSpooked = true;
    }
}
