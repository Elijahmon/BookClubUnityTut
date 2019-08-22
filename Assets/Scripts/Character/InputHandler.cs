using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InputHandler : MonoBehaviour
{
    #region References
    [SerializeField]
    PlayerController playerController;
    #endregion

    #region Input
    float movementInput;
    bool jump;
    bool shoot;
    #endregion

    #region Unity
    void Start()
    {
        
    }

    void Update()
    {
        movementInput = Input.GetAxis("HorizontalMovement");
        jump = Input.GetAxis("Jump") > 0 ? true : false;
        shoot = Input.GetAxis("Shoot") > 0 ? true : false;
        
    }

    private void FixedUpdate()
    {
        playerController.ProcessMovementInput(movementInput);
        playerController.ProcessJumpInput(jump);
        playerController.ProcessShootInput(shoot);
    }
    #endregion
}
