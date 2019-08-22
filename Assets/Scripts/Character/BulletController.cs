using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletController : MonoBehaviour
{
    [SerializeField]
    float lifeTime;
    [SerializeField]
    float speed;

    PlayerController _player;
    Transform poolParent;
    List<BulletController> pool;

    float lifeTimer;
    int direction;
   
    // Start is called before the first frame update
    public void Init(PlayerController player)
    {
        _player = player;
    }

    // Update is called once per frame
    void Update()
    {
        if (lifeTimer > 0)
        {
            if (direction > 0)
            {
                transform.position = new Vector3(transform.position.x + speed, transform.position.y, transform.position.z);
            }
            else
            {
                transform.position = new Vector3(transform.position.x - speed, transform.position.y, transform.position.z);
            }
            lifeTimer--;
        }
        else
        {
            Deactivate();
        }
        
    }

    public void Activate(Transform pooler, List<BulletController> bulletpool, int currentDirection)
    {
        direction = currentDirection;
        transform.parent = null;
        poolParent = pooler;
        pool = bulletpool;
        lifeTimer = lifeTime;
        gameObject.SetActive(true);
    }

    public void Deactivate()
    {
        gameObject.SetActive(false);
        transform.SetParent(poolParent, false);
        pool.Add(this);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.gameObject.tag == "Enemy")
        {
            GameStateManager.instance.DealDamage(collision.GetComponent<EnemyController>(), _player);
            Deactivate();
        }
    }
}
