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
    private float lowMovementThreshold = 0.4f;  // Minimum distance before considering it low movement
    private float lowMovementTime = 0f;  // Timer to track how long the monster is moving slowly

    // Footsteps AudioSource
    public AudioSource footstepsAudioSource;

    private void Start()
    {
        navMeshAgent = GetComponent<NavMeshAgent>();
        lastPosition = transform.position;
        Teleport();
    }

    private void Update()
    {
        FacePlayer();

        // Calculate distance to player
        float distanceToPlayer = Vector3.Distance(player.transform.position, transform.position);

        // Teleport if necessary based on distance to player or if stuck
        if (seen && !isPlayerLooking && (distanceToPlayer <= 75 || stuckTimer >= stuckThreshold && distanceToPlayer >= 40))
        {
            Teleport();
            stuckTimer = 0f; 
        }

        // Check if the player is looking at the monster
        CheckIfPlayerIsLooking();

        // Move toward the player unless the player is looking
        if (!isPlayerLooking)
        {
            navMeshAgent.speed = 10f;
            MoveTowardPlayer();
        }
        else if (isPlayerLooking)
        {
            navMeshAgent.speed = 0f;
        }

        if (IsMovingVeryLittle() && !isPlayerLooking)
        {
            lowMovementTime += Time.deltaTime;
            if (lowMovementTime >= stuckThreshold)
            {
                Teleport();
                lowMovementTime = 0f;
            }
        }
        else
        {
            lowMovementTime = 0f; 
        }

        if (Vector3.Distance(lastPosition, transform.position) < 0.1f)
        {
            stuckTimer += Time.deltaTime;
        }
        else
        {
            stuckTimer = 0f;
        }

        HandleFootstepsSound();

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

    private bool IsMovingVeryLittle()
    {
        float distanceMoved = Vector3.Distance(lastPosition, transform.position);

        return distanceMoved < lowMovementThreshold;
    }

    // Function to handle footsteps sound
    private void HandleFootstepsSound()
    {
        if (navMeshAgent.velocity.magnitude > 0.1f && !footstepsAudioSource.isPlaying) // If the monster is moving and the sound is not already playing
        {
            footstepsAudioSource.Play();  // Play the footsteps sound
        }
        else if (navMeshAgent.velocity.magnitude <= 0.1f && footstepsAudioSource.isPlaying) // If the monster stops moving and the sound is playing
        {
            footstepsAudioSource.Stop();  // Stop the footsteps sound
        }
    }
}
