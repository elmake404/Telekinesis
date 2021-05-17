using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum PlayerState
{
    playerIsNormal,
    playerIsMove,
    playerIsDead
}
public class PlayerController : MonoBehaviour
{
    public PlayerMove playerMove;
    private PlayerState currentPlayerState = PlayerState.playerIsNormal;
    //private Transform currentPlayerLoc;
    //private Transform destinationLoc;

    private void SwitchState(PlayerState state)
    {
        switch (state)
        {
            case PlayerState.playerIsNormal:
                break;
            case PlayerState.playerIsMove:
                InitMove();
                break;
            case PlayerState.playerIsDead:
                break;
        
        }
    }

    public void SwitchStateAction(PlayerState state)
    {
        if (state == currentPlayerState) { return; }

        currentPlayerState = state;
        SwitchState(currentPlayerState);
    }

    /*public void SetCurrentAndDestinationLoc(Transform current, Transform destination)
    {
        currentPlayerLoc = current;
        destinationLoc = destination;
    }*/

    private void InitMove()
    {
        Vector3 currentPlayerLoc = transform.position;
        Vector3 destinationLoc = currentPlayerLoc;
        destinationLoc.z += -9.0f;
        playerMove.SetCurrentLocateAndDestination(currentPlayerLoc, destinationLoc);
        playerMove.EnablePlayerMove();
        
    }
    
}

