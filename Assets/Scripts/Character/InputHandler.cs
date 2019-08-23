using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InputHandler : MonoBehaviour
{
    #region References
    PlayerController playerController;
    #endregion

    #region Input
    float movementInput;
    bool jump;
    bool shoot;
    bool changeFireMode;
    bool tryAgain;
    #endregion

    
    public void Init(PlayerController p)
    {
        playerController = p;
    }

    #region Unity
    void Update()
    {
        movementInput = Input.GetAxis("HorizontalMovement");
        jump = Input.GetAxis("Jump") > 0 ? true : false;
        shoot = Input.GetAxis("Shoot") > 0 ? true : false;
        changeFireMode = Input.GetAxis("ChangeFireMode") > 0 ? true : false;
        tryAgain = Input.GetAxis("Submit") > 0 ? true : false;
    }

    private void FixedUpdate()
    {
        if (playerController != null)
        {
            playerController.ProcessMovementInput(movementInput);
            playerController.ProcessJumpInput(jump);
            playerController.ProcessShootInput(shoot);
            playerController.ProcessChangeFireModeInput(changeFireMode);
        }
        GameStateManager.instance.ProcessTryAgainInput(tryAgain);
    }
    #endregion
}
