using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Level : MonoBehaviour
{
    public int levelNumber;
    public int wavePause = 5;
    public GameObject[] enemyPrefabs;
    public GameObject[] upgradePrefabs;

    internal void SpawnUpgrade(int randint)
    {
        Instantiate(upgradePrefabs[randint], Vector3.zero, Quaternion.identity);
    }

    public void CreateNewEnemy(Tuple<int, float, float> subWave)
    {
        var enemyPrefab = enemyPrefabs[subWave.Item1];
        var position = new Vector3(subWave.Item2, subWave.Item3, 0);

        GameObject EnemyGO = Instantiate(enemyPrefab, position, Quaternion.identity);
        Enemy enemy = EnemyGO.GetComponent<Enemy>();
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnEnable()
    {
        // Bitno samo kod debugiranja, kad počnemo iz nekog nivoa
        //Game.Instance.level = this;
    }
}
