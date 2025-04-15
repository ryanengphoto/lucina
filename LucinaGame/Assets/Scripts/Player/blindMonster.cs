using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.SceneManagement;

public class BlindMonsterAI : MonoBehaviour
{
    [Header("Patrol Settings")]
    public List<Transform> patrolPoints;
    public float waitAtWaypointTime = 2f;

    [Header("Hearing Settings")]
    public float hearRadius = 10f;
    public float investigateTime = 5f;

    [Header("Movement Settings")]
    public float randomMoveRadius = 3f;
    public float randomMoveInterval = 3f;
    public float moveToWaypointThreshold = 0.5f;

    private NavMeshAgent agent;
    private GameObject player;
    public Animator animator;
    public bool playerNoise = false;
    private PlayerMovement playerController;
    private Transform currentTarget;
    private bool isInvestigating = false;
    private bool onTopFloor = false;
    private bool isWalking = false;

    public AudioSource walkingAudio;
    public AudioSource runningAudio;
    public AudioSource breathingAudio;
    public AudioSource heartBeat;
    public AudioSource jumpscareSound;
    public float sprintSpeed = 6f;
    public float patrolSpeed = 3.5f;
    private bool chasing = false;
    public GameObject jumpscareImage;
    float stuckTimer = 0f;
    float lastCheckTime = 0f;
    Vector3 lastPosition = Vector3.zero;

    public int topEnd;
    public int bottomEnd;

    private void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        player = GameObject.FindGameObjectWithTag("Player");
        playerController = player.GetComponent<PlayerMovement>();
        PickNextPatrolPoint();
        StartCoroutine(PatrolRoutine());
    }

    private void Update()
    {
        float speed = agent.velocity.magnitude;
        bool isRunning = agent.speed >= sprintSpeed;

        if (!chasing && heartBeat.isPlaying)
        {
            heartBeat.Stop();
        }

        if (speed > 0.1f && !isWalking)
        {
            animator.SetBool("walking", true);
            isWalking = true;
            if (walkingAudio != null && !walkingAudio.isPlaying)
                walkingAudio.Play();
        }
        else if (speed <= 0.1f && isWalking)
        {
            animator.SetBool("walking", false);
            isWalking = false;
            if (walkingAudio != null && walkingAudio.isPlaying)
                walkingAudio.Stop();
        }

        walkingAudio.pitch = isRunning ? 1.5f : 0.85f;

         float distanceToPlayer = Vector3.Distance(transform.position, player.transform.position);
        if (playerController.makingNoise && distanceToPlayer <= hearRadius)
        {
            chasing = true;
            isInvestigating = true;
            Vector3 lastHeardPosition = player.transform.position;

            if (breathingAudio != null && breathingAudio.isPlaying)
                breathingAudio.Stop();

            if (runningAudio != null && !runningAudio.isPlaying)
                runningAudio.Play();

            if (!heartBeat.isPlaying)
                heartBeat.Play();

            agent.speed = sprintSpeed;

            StopAllCoroutines();
            agent.SetDestination(lastHeardPosition);
        }
        else if (!playerController.makingNoise && isInvestigating)
        {
            isInvestigating = false;
            Vector3 lastHeardPosition = player.transform.position;
            StartCoroutine(InvestigateSound(lastHeardPosition));
        }
        if (Time.time - lastCheckTime > 1f)
        {
            lastCheckTime = Time.time;
            if ((transform.position - lastPosition).sqrMagnitude < 0.01f && agent.hasPath)
            {
                stuckTimer += 1f;
                if (stuckTimer > 2f) // stuck for 2+ seconds
                {
                    agent.ResetPath();
                    PickNextPatrolPoint();
                    StartCoroutine(PatrolRoutine());
                    stuckTimer = 0f;
                }
            }
            else
            {
                stuckTimer = 0f;
            }
            lastPosition = transform.position;
        }
    }

    private void PickNextPatrolPoint()
    {
        bool switchFloors = Random.value < 0.25f;
        if (switchFloors) onTopFloor = !onTopFloor;

        int start = onTopFloor ? 0 : topEnd;
        int end = onTopFloor ? topEnd : bottomEnd;

        for (int attempts = 0; attempts < 10; attempts++)
        {
            int index = Random.Range(start, end);
            Transform candidate = patrolPoints[index];

            NavMeshPath path = new NavMeshPath();
            if (agent.CalculatePath(candidate.position, path) && path.status == NavMeshPathStatus.PathComplete)
            {
                currentTarget = candidate;
                break;
            }
        }

        float distanceToPlayer = Vector3.Distance(transform.position, player.transform.position);
        if (distanceToPlayer <= hearRadius && Random.value < 0.05f)
        {
            StopAllCoroutines();
            StartCoroutine(InvestigateSound(player.transform.position));
        }
    }

    private IEnumerator PatrolRoutine()
    {
        agent.speed = patrolSpeed;

        while (true)
        {
            if (currentTarget == null) yield break;

            agent.SetDestination(currentTarget.position);

            if (Mathf.Abs(transform.position.y - currentTarget.position.y) > 1f)
            {
                // Floor switch detected, skip random movement
                agent.SetDestination(currentTarget.position);
            }
            else
            {
                StartCoroutine(RandomMove());
            }

            
            if (currentTarget != null && IsSameFloor(transform.position, currentTarget.position))
                StartCoroutine(RandomMove());

            while (agent.remainingDistance > moveToWaypointThreshold)
                yield return null;

            yield return new WaitForSeconds(waitAtWaypointTime);
            PickNextPatrolPoint();
        }
    }

    private bool IsSameFloor(Vector3 a, Vector3 b)
    {
        return Mathf.Abs(a.y - b.y) < 1f; // Adjust the threshold based on your floor height
    }

    private IEnumerator RandomMove()
    {
        while (true)
        {
            if (agent.pathPending || agent.remainingDistance <= moveToWaypointThreshold)
                yield return null;

            if (Random.value < 0.1f)
            {
                Vector3 randomDirection = Random.insideUnitSphere * randomMoveRadius + transform.position;
                if (NavMesh.SamplePosition(randomDirection, out NavMeshHit hit, randomMoveRadius, NavMesh.AllAreas))
                    agent.SetDestination(hit.position);
            }

            yield return new WaitForSeconds(randomMoveInterval);
        }
    }

    private IEnumerator InvestigateSound(Vector3 location)
    {
        agent.speed = sprintSpeed;

        float timer = 0f;

        investigateTime = Random.Range(2, 5);

        while (timer < investigateTime)
        {
            Vector3 randomOffset = Random.insideUnitSphere * 3f;
            randomOffset.y = 0;
            Vector3 searchPoint = location + randomOffset;

            if (NavMesh.SamplePosition(searchPoint, out NavMeshHit hit, 3f, NavMesh.AllAreas))
                agent.SetDestination(hit.position);

            while (agent.pathPending || agent.remainingDistance > 1f)
                yield return null;

            yield return new WaitForSeconds(1f);
            timer += 1f;
        }

        if (runningAudio != null)
            StartCoroutine(FadeOutAudio(runningAudio, 1f));

        isInvestigating = false;
        chasing = false;
        agent.speed = patrolSpeed;
        PickNextPatrolPoint();
        StartCoroutine(PatrolRoutine());
    }

    private IEnumerator FadeOutAudio(AudioSource audioSource, float duration)
    {
        float startVolume = audioSource.volume;

        float time = 0f;
        while (time < duration)
        {
            audioSource.volume = Mathf.Lerp(startVolume, 0, time / duration);
            time += Time.deltaTime;
            yield return null;
        }

        audioSource.Stop();

        if (breathingAudio != null && !breathingAudio.isPlaying)
            breathingAudio.Play();

        audioSource.volume = startVolume;
    }
}
