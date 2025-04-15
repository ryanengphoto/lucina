using UnityEngine;
using System.Collections;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;
    public int momentos = 0;
    public GameObject monster;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);

            if (monster != null)
            {
                monster.SetActive(false);
            }
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

        if (monster != null)
        {
            monster.SetActive(true);
        }
    }
}
