using UnityEngine;

public class PlayerStateJump : MonoBehaviour, IPlayerState
{
    private PlayerController _playerController;
    
    public void Enter(PlayerController playerController)
    {
        _playerController = playerController;
        _playerController.Animator.SetTrigger("Jump2");
        _playerController.Jump();
    }

    public void Update()
    {
        var distanceToGround = _playerController.GetDistanceToGround();
        if (distanceToGround < 0.1f)
        {
            _playerController.SetState(PlayerState.Idle);
        }
        else
        {
            _playerController.Animator.SetFloat("GroundDistance", _playerController.GetDistanceToGround());
        }
        
    }

    public void Exit()
    {
        _playerController = null;
        
    }
}
