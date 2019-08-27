using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletCasingController : MonoBehaviour
{
    [SerializeField]
    Rigidbody2D _rigidbody;
    Transform pool;

    bool alive;

    public void Init(Transform poolParent)
    {
        alive = false;
        pool = poolParent;
        gameObject.SetActive(false);
    }

    public void Fire(Vector2 direction, float speed)
    {
        alive = true;
        transform.parent = null;
        gameObject.SetActive(true);
        _rigidbody.AddForce(direction * speed);
    }

    public void Retrieve()
    {
        alive = false;
        gameObject.SetActive(false);
        transform.SetParent(pool, false);
        transform.localPosition = Vector2.zero;
    }

    public bool Alive()
    {
        return alive;
    }
}
