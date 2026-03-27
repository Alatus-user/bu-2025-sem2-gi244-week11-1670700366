using System.Collections;
using UnityEngine;

public class SpawnManager : MonoBehaviour
{
    public Transform[] spawnPoints;
    public GameObject enemyPrefab;

    void Start()
    {
        //InvokeRepeating(nameof(RandomSpawn), 2f, 3f);
        StartCoroutine(SpawnRoutine());
    }

    void RandomSpawn()
    {
        var index = Random.Range(0, spawnPoints.Length);
        var spawnPoint = spawnPoints[index];

        Instantiate(enemyPrefab, spawnPoint.position, Quaternion.identity);


    }


    IEnumerator SpawnRoutine()
    {
        yield return new WaitForSeconds(5f);
        while (true)
        {
            RandomSpawn();
            yield return new WaitForSeconds(3f);
        }
    }


    /*IEnumerator Hello()
    {
        Debug.Log("Hello" + Time.frameCount);
        yield return null;
    }

    IEnumerator GoodBye()
    {
        yield return new WaitForSeconds(1f);
        Debug.Log("Goodbye" + Time.frameCount  + " " + Time.time);
    }*/
}
