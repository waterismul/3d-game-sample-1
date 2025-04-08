using UnityEngine;

public class PlayerStateIdle : MonoBehaviour, IPlayerState
{
    private PlayerController _playerController;
    
    public void Enter(PlayerController playerController)//한번만 해야할 일
    {
        _playerController = playerController;
        _playerController.Animator.SetBool("Idle", true);
    }

    public void Update()//지속적으로 해야할 일
    {
        var inputVertical = Input.GetAxis("Vertical");
        var inputHorizontal = Input.GetAxis("Horizontal");
        
        //이동
        if (inputVertical != 0 || inputHorizontal != 0)
        {
            _playerController.Rotate(inputVertical, inputHorizontal);//회전 처리
            _playerController.SetState(PlayerState.Move);
            return;
        }
        
        //점프
        if (Input.GetKeyDown(KeyCode.Space))
        {
            _playerController.SetState(PlayerState.Jump);
            return;
        }
        
        //공격
        if (Input.GetButtonDown("Fire1"))
        {
            _playerController.SetState(PlayerState.Attack);
            return;
        }
    }

    public void Exit()
    {
        _playerController.Animator.SetBool("Idle", false);
        _playerController = null;
    }
}
