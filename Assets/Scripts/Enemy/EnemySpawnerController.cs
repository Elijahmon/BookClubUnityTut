using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemySpawnerController : MonoBehaviour
{
    [SerializeField]
    GameObject enemyPrefab;

    public void SpawnOne(List<EnemyController> enemiesList)
    {
        EnemyController e = Instantiate<GameObject>(enemyPrefab, transform.position, enemyPrefab.transform.rotation).GetComponent<EnemyController>();
        e.Init();
        enemiesList.Add(e);
    }

}
