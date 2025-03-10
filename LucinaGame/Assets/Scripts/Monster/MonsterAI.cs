using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class MonsterAI : MonoBehaviour
{
    public Transform[] teleportSpots;
    public GameObject player;
    private Vector3 targetPosition;
    private bool isPlayerLooking = false;
    private NavMeshAgent navMeshAgent;
    private bool seen = false;
    public LookAtMonster lookatmonsterScript;
    private Transform prevSpot;
    private Vector3 lastPosition;
    private float stuckTimer = 0f;
    private float stuckThreshold = 5f;

    private void Start()
    {
        navMeshAgent = GetComponent<NavMeshAgent>();
        lastPosition = transform.position;
        Teleport();
    }

    private void Update()
    {
        FacePlayer();
        if (Vector3.Distance(lastPosition, transform.position) < 0.1f)
        {
            stuckTimer += Time.deltaTime;
        }
        else
        {
            stuckTimer = 0f;
        }

        float distanceToPlayer = Vector3.Distance(player.transform.position, transform.position);

        if (seen && !isPlayerLooking && (distanceToPlayer <= 75 || stuckTimer >= stuckThreshold && distanceToPlayer >= 40))
        {
            Teleport();
            stuckTimer = 0f; 
        }

        CheckIfPlayerIsLooking();

        if (!isPlayerLooking)
        {
            MoveTowardPlayer();
        }
        else if (isPlayerLooking)
        {
            navMeshAgent.speed = 0f;
        }
        else
        {
            navMeshAgent.speed = 10f;
        }

        lastPosition = transform.position;
    }

    private void CheckIfPlayerIsLooking()
    {
        if (lookatmonsterScript.looking)
        {
            isPlayerLooking = true;
            seen = true;
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

        int startRange = GameManager.Instance.momentos >= 2 ? 4 : 0; 
        int endRange = GameManager.Instance.momentos >= 2 ? teleportSpots.Length : 8; 

        do
        {
            randomSpot = teleportSpots[Random.Range(startRange, endRange)];
        }
        while (randomSpot == prevSpot);

        targetPosition = randomSpot.position;
        prevSpot = randomSpot;
        transform.position = targetPosition;
        seen = false;
    }

    private void FacePlayer()
    {
        Vector3 directionToPlayer = player.transform.position - transform.position;
        directionToPlayer.y = 0; 
        
        Quaternion targetRotation = Quaternion.LookRotation(directionToPlayer);

        transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.deltaTime * 5f); 
    }
}
