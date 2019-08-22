using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterAnimatorEventHandler : MonoBehaviour
{
    [SerializeField]
    PlayerController _controller;

    void OnShoot()
    {
        _controller.FireBullet();
    }

    void DeadFrame()
    {
        _controller.HandleDeadFrame();
    }
}
