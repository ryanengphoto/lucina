using UnityEngine;
using System.Collections;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;
    public int momentos = 0;
    public GameObject monster;

    private void Awake()
    {
        monster.SetActive(false);
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void UpdateMomentos(int count)
    {
        momentos += count;
        
        if (momentos == 1)
        {
            StartCoroutine(SpawnMonsterWithDelay());
        }
    }

    private IEnumerator SpawnMonsterWithDelay()
    {
        yield return new WaitForSeconds(10);
        monster.SetActive(true);
    }
}
