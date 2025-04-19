using System.Collections;
using UnityEngine;
using UnityEngine.AI;

public class AngelAI : MonoBehaviour
{
    [SerializeField] private Transform player;
    [SerializeField] private float activateDistance = 30f;
    [SerializeField] private AudioSource footstepsAudioSource;
    [SerializeField] private LayerMask obstacleMask;
    private Coroutine fadeOutRoutine;


    private NavMeshAgent agent;
    private bool canActivate = false;
    private bool isBeingLookedAt = false;

    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        agent.updateRotation = false;
        agent.updateUpAxis = true;
    }

    void Update()
    {
        float distance = Vector3.Distance(transform.position, player.position);
        canActivate = distance <= activateDistance;

        bool isBlocked = IsObstructed();

        if (!isBeingLookedAt || isBlocked)
        {
            if (canActivate)
            {
                agent.speed = 6f;
                FacePlayer();
                MoveTowardPlayer();
            } else {
                FacePlayer();
            }
        }
        else
        {
            agent.speed = 0f;
            agent.ResetPath();
            agent.velocity = Vector3.zero;
        }

        HandleFootstepsSound();
    }

    private bool IsObstructed()
    {
        Vector3 direction = player.position - transform.position;
        float distance = direction.magnitude;

        if (Physics.Raycast(transform.position, direction.normalized, out RaycastHit hit, distance, obstacleMask))
        {
            return true;
        }

        return false;
    }

    private void HandleFootstepsSound()
    {
        if (agent.velocity.magnitude > 0.1f)
        {
            if (!footstepsAudioSource.isPlaying)
            {
                footstepsAudioSource.time = Random.Range(0f, footstepsAudioSource.clip.length);
                footstepsAudioSource.volume = 0.1f;
                footstepsAudioSource.Play();
            }

            if (fadeOutRoutine != null)
            {
                StopCoroutine(fadeOutRoutine);
                fadeOutRoutine = null;
                footstepsAudioSource.volume = 0.1f;
            }
        }
        else if (agent.velocity.magnitude <= 0.1f && footstepsAudioSource.isPlaying && fadeOutRoutine == null)
        {
            fadeOutRoutine = StartCoroutine(FadeOut(footstepsAudioSource, 0.5f));
        }
    }


    public IEnumerator FadeOut(AudioSource audioSource, float duration)
    {
        float startVolume = audioSource.volume;

        float time = 0f;
        while (time < duration)
        {
            time += Time.deltaTime;
            audioSource.volume = Mathf.Lerp(startVolume, 0f, time / duration);
            yield return null;
        }

        audioSource.volume = 0f;
        audioSource.Stop();
        fadeOutRoutine = null;
    }


    private void MoveTowardPlayer()
    {
        Vector3 targetPosition = player.position;
        NavMeshHit hit;

        if (NavMesh.SamplePosition(targetPosition, out hit, 2.0f, NavMesh.AllAreas))
        {
            agent.SetDestination(hit.position);
        }
        else
        {
            agent.ResetPath();
        }
    }

    private void FacePlayer()
    {
        Vector3 directionToPlayer = player.position - transform.position;
        directionToPlayer.y = 0;

        Quaternion targetRotation = Quaternion.LookRotation(directionToPlayer);
        transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.deltaTime * 5f);
    }

    public void SetLookingAt(bool isLooking)
    {
        isBeingLookedAt = isLooking;
    }

    public void SetPlayer(Transform newPlayer)
    {
        player = newPlayer;
    }
}
