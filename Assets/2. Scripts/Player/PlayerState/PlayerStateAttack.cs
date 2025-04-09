using UnityEngine;

public class PlayerStateAttack : MonoBehaviour, IPlayerState
{
    private PlayerController _playerController;
    public bool IsAttacking { get; set; }
    
    public void Enter(PlayerController playerController)
    {
        _playerController = playerController;
        _playerController.Animator.SetTrigger("Attack");
    }

    public void Update()
    {
        if (Input.GetButtonDown("Fire1") && _playerController.IsGrounded && !IsAttacking)
        {
            _playerController.Animator.SetTrigger("Attack");//SetTrigger라서 동작이 끝남을 알려주기 위해 Add Behaviour를 해서 PlayerAnimatorStateAttack을 추가해서 여기서 처리
            return;
        }
    }

    public void Exit()
    {
        _playerController = null;
    }
}
