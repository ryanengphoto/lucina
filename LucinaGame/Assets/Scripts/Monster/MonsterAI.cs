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
    private float lowMovementThreshold = 0.4f;
    private float lowMovementTime = 0f; 
    private bool isDisabled = false;
    private float cooldownDuration = 0f;
    private bool cooldownActive = false;

    public AudioSource footstepsAudioSource;

    private void Start()
    {
        navMeshAgent = GetComponent<NavMeshAgent>();
        lastPosition = transform.position;
        Teleport();
    }

    private void Update()
    {
        if (isDisabled)
        {
            navMeshAgent.speed = 0f;
            HandleFootstepsSound();
            return;
        }

        FacePlayer();

        float distanceToPlayer = Vector3.Distance(player.transform.position, transform.position);

        if (seen && !isPlayerLooking && (distanceToPlayer <= 75 || stuckTimer >= stuckThreshold && distanceToPlayer >= 40))
        {
            StartCooldownAndDisable();
            stuckTimer = 0f;
        }

        CheckIfPlayerIsLooking();

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

        int startRange = GameManager.Instance.momentos >= 4 ? 4 : 0; 
        int endRange = GameManager.Instance.momentos >= 4 ? teleportSpots.Length : 8; 

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

    private void HandleFootstepsSound()
    {
        if (navMeshAgent.velocity.magnitude > 0.1f && !footstepsAudioSource.isPlaying) 
        {
            footstepsAudioSource.Play();
        }
        else if (navMeshAgent.velocity.magnitude <= 0.1f && footstepsAudioSource.isPlaying) 
        {
            footstepsAudioSource.Stop(); 
        }
    }

    private float GetCooldownDuration()
    {
        int mementos = GameManager.Instance.momentos;

        if(mementos >= 5){
            return 0f;
        }
        float baseMin = 5f;
        float baseMax = 10f;
        
        float minCooldown = Mathf.Lerp(baseMin, 1.5f, (mementos - 1) / 5f);
        float maxCooldown = Mathf.Lerp(baseMax, 3f, (mementos - 1) / 5f);
        
        return Random.Range(minCooldown, maxCooldown);
    }

    private void StartCooldownAndDisable()
    {
        if (!cooldownActive)
        {
            isDisabled = true;
            cooldownDuration = GetCooldownDuration();
            cooldownActive = true;

            navMeshAgent.enabled = false;
            transform.position = Vector3.zero;

            StartCoroutine(EnableAfterCooldown());
        }
    }

    private IEnumerator EnableAfterCooldown()
    {
        yield return new WaitForSeconds(cooldownDuration);

        Teleport();
        navMeshAgent.enabled = true;

        isDisabled = false;
        cooldownActive = false;
    }
}
