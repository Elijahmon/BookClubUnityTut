using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DebrisManager : MonoBehaviour
{
    public static DebrisManager instance;

    [SerializeField]
    int bulletHoleCap;
    [SerializeField]
    GameObject bulletholePrefab;
    List<GameObject> bulletHoles;
    

    int bulletHoleIndex;

    public void Init()
    {
        instance = this;
        bulletHoles = new List<GameObject>();
        bulletHoleIndex = 0;

        for(int i = bulletHoleCap; i > 0; i--)
        {
            GameObject bulletHole = Instantiate<GameObject>(bulletholePrefab, transform);
            bulletHole.SetActive(false);
            bulletHoles.Add(bulletHole);
        }
    }

    public void SpawnBulletHole(Vector3 position)
    {
        if (bulletHoleIndex > bulletHoles.Count-1)
        {
            bulletHoleIndex = 0;
        }
        GameObject b = bulletHoles[bulletHoleIndex];
        b.SetActive(false);
        b.transform.position = position;
        b.SetActive(true);

        bulletHoleIndex++;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
