using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MonsterSpawner : MonoBehaviour
{
    public GameObject monster;
    private bool hasStartedSpawn = false;
    void Start()
    {
        monster.SetActive(false);
    }
    
    void Update()
    {
        if (!hasStartedSpawn && GameManager.Instance != null && GameManager.Instance.momentos == 1)
        {
            hasStartedSpawn = true;
            StartCoroutine(SpawnMonsterWithDelay());
        }
    }

    private IEnumerator SpawnMonsterWithDelay()
    {
        yield return new WaitForSeconds(15);
        monster.SetActive(true);
    }
}
