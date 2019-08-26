using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyAmationEventHandler : MonoBehaviour
{
    [SerializeField]
    EnemyController _enemy;

    void DamageFrame()
    {
        _enemy.HandleDamageFrame();
    }

    void DeadFrame()
    {
        _enemy.HandleDeadFrame();
    }

    void DeactivateCollider()
    {

    }

    void AlertCompleted()
    {
        _enemy.ResetAlert();
    }

    void HitCompleted()
    {
        _enemy.ResetHit();
    }
}
