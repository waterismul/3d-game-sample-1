using UnityEngine;

public class PlayerStateIdle : MonoBehaviour, IPlayerState
{
    private PlayerController _playerController;
    
    public void Enter(PlayerController playerController)
    {
        _playerController = playerController;
    }

    public void Update()
    {
        
    }

    public void Exit()
    {
        _playerController = null;
    }
}
