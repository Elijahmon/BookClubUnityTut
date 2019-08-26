using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletController : MonoBehaviour
{
    [SerializeField]
    Animator _anim;
    [SerializeField]
    float lifeTime;
    [SerializeField]
    float speed;

    PlayerController _player;
    Transform poolParent;
    List<BulletController> pool;

    float lifeTimer;
    Vector2 direction;
    bool hit;
   
    // Start is called before the first frame update
    public void Init(PlayerController player)
    {
        
        _player = player;
        hit = false;
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if (lifeTimer > 0 && !hit)
        {
            if (direction.x > 0)
            {
                transform.position = new Vector2(transform.position.x + (direction.x * speed/2), transform.position.y + (direction.y * speed/2));
            }
            else
            {
                transform.position = new Vector2(transform.position.x - (speed / 2), transform.position.y + (direction.y * speed / 2));
            }
            lifeTimer--;
        }
        else
        {
            Deactivate();
        }
         
    }

    public void Activate(Transform pooler, List<BulletController> bulletpool, Vector2 currentDirection)
    {
        direction = currentDirection;
        transform.parent = null;
        poolParent = pooler;
        pool = bulletpool;
        lifeTimer = lifeTime;
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg -90;
        transform.rotation = Quaternion.AngleAxis(angle, Vector3.forward);
        gameObject.SetActive(true);
    }

    public void OnHit()
    {
        transform.localScale = new Vector3(.5f, .5f, 1);
        hit = true;
        _anim.SetTrigger("Hit");
    }

    public void Deactivate()
    {
        transform.localScale = new Vector3(0.05f, 0.05f, 1);
        hit = false;
        _anim.ResetTrigger("Hit");
        gameObject.SetActive(false);
        transform.SetParent(poolParent, false);
        pool.Add(this);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "Enemy")
        {
            if (collision.GetComponent<EnemyController>().IsAlive())
            {
                GameStateManager.instance.DealDamage(collision.GetComponent<EnemyController>(), _player);
                OnHit();
            }
            
        }
        else if (collision.gameObject.tag == "Ground")
        {
            OnHit();
        }
    }
}
