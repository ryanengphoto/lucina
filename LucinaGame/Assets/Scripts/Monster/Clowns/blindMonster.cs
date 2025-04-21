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

    private float stuckTimer = 0f;
    private float lastCheckTime = 0f;
    private Vector3 lastPosition = Vector3.zero;

    public int topEnd;
    public int bottomEnd;

    private Coroutine randomMoveCoroutine;

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
            heartBeat.Stop();

        if (speed > 0.1f && !isWalking)
        {
            animator.SetBool("walking", true);
            isWalking = true;
            if (walkingAudio && !walkingAudio.isPlaying)
                walkingAudio.Play();
        }
        else if (speed <= 0.1f && isWalking)
        {
            animator.SetBool("walking", false);
            isWalking = false;
            if (walkingAudio && walkingAudio.isPlaying)
                walkingAudio.Stop();
        }

        if (walkingAudio)
            walkingAudio.pitch = isRunning ? 1.5f : 0.85f;

        float distanceToPlayer = Vector3.Distance(transform.position, player.transform.position);
        if (playerController.makingNoise && distanceToPlayer <= hearRadius)
        {
            chasing = true;
            isInvestigating = true;
            Vector3 lastHeardPosition = player.transform.position;

            if (breathingAudio && breathingAudio.isPlaying && Time.timeScale == 1)
                breathingAudio.Stop();

            if (runningAudio && !runningAudio.isPlaying && Time.timeScale == 1)
                runningAudio.Play();

            if (!heartBeat.isPlaying && Time.timeScale == 1)
                heartBeat.Play();

            agent.speed = sprintSpeed;

            StopAllCoroutines();
            if (randomMoveCoroutine != null) StopCoroutine(randomMoveCoroutine);
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

            if (!chasing && !isInvestigating && agent.hasPath)
            {
                float movedDistance = (transform.position - lastPosition).sqrMagnitude;

                if (movedDistance < 0.05f)
                {
                    stuckTimer += 1f;

                    if (stuckTimer >= 2f)
                    {
                        agent.ResetPath();
                        PickNextPatrolPoint();
                        StopAllCoroutines();
                        if (randomMoveCoroutine != null) StopCoroutine(randomMoveCoroutine);
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
        if (distanceToPlayer <= hearRadius && Random.value < 0.05f && !playerController.outside)
        {
            TrySetDestination(currentTarget.position);
        }
    }

    private IEnumerator PatrolRoutine()
    {
        agent.speed = patrolSpeed;

        while (true)
        {
            if (currentTarget == null) yield break;

            agent.SetDestination(currentTarget.position);

            if (Mathf.Abs(transform.position.y - currentTarget.position.y) < 1f)
            {
                if (randomMoveCoroutine != null)
                    StopCoroutine(randomMoveCoroutine);
                randomMoveCoroutine = StartCoroutine(RandomMove());
            }

            float waitTimer = 0f;
            while (agent.pathPending || agent.remainingDistance > moveToWaypointThreshold)
            {
                waitTimer += Time.deltaTime;
                if (waitTimer > 10f) break;
                yield return null;
            }

            yield return new WaitForSeconds(waitAtWaypointTime);
            PickNextPatrolPoint();
        }
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
        investigateTime = Random.Range(2f, 5f);

        while (timer < investigateTime)
        {
            Vector3 randomOffset = Random.insideUnitSphere * 3f;
            randomOffset.y = 0;
            Vector3 searchPoint = location + randomOffset;

            if (NavMesh.SamplePosition(searchPoint, out NavMeshHit hit, 3f, NavMesh.AllAreas))
                agent.SetDestination(hit.position);

            float waitTimer = 0f;
            while (agent.pathPending || agent.remainingDistance > 1f)
            {
                waitTimer += Time.deltaTime;
                if (waitTimer > 10f) break;
                yield return null;
            }

            yield return new WaitForSeconds(1f);
            timer += 1f;
        }

        if (runningAudio)
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

        if (breathingAudio && !breathingAudio.isPlaying)
            breathingAudio.Play();

        audioSource.volume = startVolume;
    }

    private bool TrySetDestination(Vector3 destination)
    {
        NavMeshPath path = new NavMeshPath();
        if (agent.CalculatePath(destination, path) && path.status == NavMeshPathStatus.PathComplete)
        {
            agent.SetDestination(destination);
            return true;
        }
        else if (NavMesh.SamplePosition(destination, out NavMeshHit hit, 3f, NavMesh.AllAreas))
        {
            if (agent.CalculatePath(hit.position, path) && path.status == NavMeshPathStatus.PathComplete)
            {
                agent.SetDestination(hit.position);
                return true;
            }
        }
        return false;
    }

    private bool IsSameFloor(Vector3 a, Vector3 b)
    {
        return Mathf.Abs(a.y - b.y) < 1f;
    }
}
