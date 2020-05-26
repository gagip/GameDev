using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MonsterSpawner : MonoBehaviour {

    public GameObject spawnMonster;

    public List<GameObject> MonsterList = new List<GameObject>();

    public int spawnMaxCount = 50;

    private void Start()
    {
        InvokeRepeating("SpawnMonster", 3f, 5f);
    }

    void SpawnMonster()
    {
        if(MonsterList.Count > spawnMaxCount)
        {
            return;
        }

        Vector3 spawnPos = new Vector3(Random.Range(-100.0f, 100.0f), 1000.0f, Random.Range(-100.0f, 100.0f));

        Ray ray = new Ray(spawnPos, Vector3.down);
        RaycastHit hit = new RaycastHit();

        if (Physics.Raycast(ray, out hit, Mathf.Infinity) == true)
        {
            spawnPos.y = hit.point.y;
        }

        GameObject newMonster = Instantiate(spawnMonster, spawnPos, Quaternion.identity);
        MonsterList.Add(newMonster);
    }

}
