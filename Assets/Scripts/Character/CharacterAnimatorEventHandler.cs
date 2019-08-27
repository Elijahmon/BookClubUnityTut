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

    void OnCasing()
    {
        _controller.FireCasing();
    }

    void DeadFrame()
    {
        _controller.HandleDeadFrame();
    }

    void StartSmoke()
    {
        _controller.ToggleSmoke(true);
    }
    void StopSmoke()
    {
        _controller.ToggleSmoke(false);
    }
}
