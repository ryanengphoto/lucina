using System.Collections.Generic;
using UnityEngine;

public class MementoRandomizer : MonoBehaviour
{
    [Header("Memento Prefabs (6 unique ones)")]
    public GameObject[] uniqueMementos;

    [Header("Outside Spawn Points (3)")]
    public Transform[] outsideSpots;

    [Header("Library Spawn Points (2)")]
    public Transform[] librarySpots;

    [Header("Dorms Spawn Points (7)")]
    public Transform[] dormsSpots;

    [Header("Garden Spawn Points (3)")]
    public Transform[] gardenSpots;

    [Header("Union Spawn Points (3)")]
    public Transform[] unionSpots;
    [Header("Light Poles")]
    public GameObject[] poles;

    void Start()
    {
        if (uniqueMementos.Length != 6)
        {
            return;
        }

        Dictionary<string, (Transform[], int)> locationData = new Dictionary<string, (Transform[], int)>
        {
            { "Outside", (outsideSpots, 1) },
            { "Library", (librarySpots, 1) },
            { "Dorms", (dormsSpots, 1) },
            { "Garden", (gardenSpots, 2) },
            { "Union", (unionSpots, 1) }
        };

        List<GameObject> shuffledMementos = new List<GameObject>(uniqueMementos);
        Shuffle(shuffledMementos);

        int mementoIndex = 0;

        System.Random rand = new System.Random();

        foreach (var entry in locationData)
        {
            string location = entry.Key;
            Transform[] spots = entry.Value.Item1;
            int mementosToPlace = entry.Value.Item2;

            List<int> availableIndices = new List<int>();
            for (int i = 0; i < spots.Length; i++) availableIndices.Add(i);

            for (int i = 0; i < mementosToPlace; i++)
            {
                int randIndex = rand.Next(availableIndices.Count);
                int chosenSpotIndex = availableIndices[randIndex];
                availableIndices.RemoveAt(randIndex);

                Transform spawnPoint = spots[chosenSpotIndex];
                GameObject memento = shuffledMementos[mementoIndex++];

                Instantiate(memento, spawnPoint.position, spawnPoint.rotation);

                if(location == "Outside" && chosenSpotIndex == 1){
                    poles[0].GetComponentInChildren<LightBlinker>().off = false;
                    poles[0].GetComponentInChildren<LightBlinker>().favorOn = true;
                } else if (location == "Garden" && chosenSpotIndex == 0){
                    poles[1].GetComponentInChildren<LightBlinker>().off = false;
                    poles[1].GetComponentInChildren<LightBlinker>().favorOn = true;
                } else if (location == "Garden" && chosenSpotIndex == 1){
                    poles[2].GetComponentInChildren<LightBlinker>().off = false;
                    poles[2].GetComponentInChildren<LightBlinker>().favorOn = true;
                } else if (location == "Garden" && chosenSpotIndex == 2){
                    poles[3].GetComponentInChildren<LightBlinker>().off = false;
                    poles[3].GetComponentInChildren<LightBlinker>().favorOn = true;
                    poles[4].GetComponentInChildren<LightBlinker>().off = false;
                    poles[4].GetComponentInChildren<LightBlinker>().favorOn = true;
                } else if (location == "Dorms" && chosenSpotIndex == 1 || location == "Dorms" && chosenSpotIndex == 0){
                    poles[6].GetComponentInChildren<LightBlinker>().off = false;
                    poles[6].GetComponentInChildren<LightBlinker>().favorOn = true;
                } else if (location == "Dorms" && chosenSpotIndex == 2 || location == "Dorms" && chosenSpotIndex == 3){
                    poles[5].GetComponentInChildren<LightBlinker>().off = false;
                    poles[5].GetComponentInChildren<LightBlinker>().favorOn = true;
                } else if (location == "Dorms" && chosenSpotIndex == 4 || location == "Dorms" && chosenSpotIndex == 6 || location == "Dorms" && chosenSpotIndex == 5){
                    poles[7].GetComponentInChildren<LightBlinker>().off = false;
                    poles[7].GetComponentInChildren<LightBlinker>().favorOn = true;
                } 
            }
        }
    }

    void Shuffle<T>(List<T> list)
    {
        System.Random rng = new System.Random();
        int n = list.Count;
        while (n > 1)
        {
            n--;
            int k = rng.Next(n + 1);
            (list[k], list[n]) = (list[n], list[k]);
        }
    }
}
