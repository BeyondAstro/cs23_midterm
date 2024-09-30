using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemySpawningLogic : MonoBehaviour
{
    [SerializeField] private GameObject[] spawnPoints;
    [SerializeField] private GameObject[] enemyPrefabs;
    [SerializeField] private int spawnPointNumber = 6;
    [SerializeField] private float timeBetweenWaves = 1f;
    [SerializeField] private bool canSpawn = true;

    private int waveSize = 6;
    private int waveIndex = 0;
    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine(Spawner());
    }
    private IEnumerator Spawner()
    {
        WaitForSeconds wait = new WaitForSeconds(timeBetweenWaves);
        while (canSpawn)
        {
            yield return wait;
            waveIndex++;
            
            int randomIndex = Random.Range(0, enemyPrefabs.Length);
            List<int> selectedIndexes = GetRandomIndexes(spawnPoints.Length, spawnPointNumber);
            foreach (int index in selectedIndexes)
            {
                for(int i = 0; i < System.Math.Ceiling((float)waveSize/spawnPointNumber); i++){
                    Instantiate(enemyPrefabs[0], spawnPoints[index].transform.position, Quaternion.identity);
                }
            }
        }
    }
    private List<int> GetRandomIndexes(int listLength, int numberOfIndexes)
    {
        List<int> indexes = new List<int>();
        HashSet<int> selected = new HashSet<int>();

        while (selected.Count < numberOfIndexes)
        {
            int randomIndex = Random.Range(0, listLength);
            if (selected.Add(randomIndex))
            {
                indexes.Add(randomIndex);
            }
        }

        return indexes;
    }
    // Update is called once per frame
}
