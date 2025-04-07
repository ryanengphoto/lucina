using UnityEngine;
using UnityEngine.SceneManagement;


public class WaterDeath : MonoBehaviour
{
    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            SceneManager.LoadScene("DEATH_SCENE");
        }
    }
}