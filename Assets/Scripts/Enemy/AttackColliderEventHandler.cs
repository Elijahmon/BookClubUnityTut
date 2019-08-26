using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AttackColliderEventHandler : MonoBehaviour
{
    [SerializeField]
    EnemyController _enemy;
    [SerializeField]
    BoxCollider2D _coll;
    [SerializeField]
    ContactFilter2D filter;

    Collider2D[] collisions;

    private void Start()
    {
        collisions = new Collider2D[100]; //TODO: Do something else maybe?
    }

    /// <summary>
    /// Checks current collisions for whether or not the player is hit
    /// </summary>
    public void CheckCollisions()
    {
        _coll.OverlapCollider(filter, collisions);

        if(collisions != null)
        {
            foreach(var coll in collisions)
            {
                if (coll != null)
                {
                    if (coll.tag == "Player") //is the player overlapping this attack collider
                    {
                        GameStateManager.instance.DealDamage(coll.GetComponent<PlayerController>(), _enemy);
                        break;
                    }
                }
            }
        }
    }

    public float GetWidth()
    {
        return _coll.size.x;
    }

    public float GetOffset()
    {
        return _coll.offset.x;
    }
}
