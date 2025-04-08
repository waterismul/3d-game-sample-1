public interface IPlayerState
{
    //해당 상태로 진입했을 때 호출되는 메서드
    void Enter(PlayerController playerController);
    
    //해당상태에 머물러 있을 때 Update주기로 호출되는 메서드
    void Update();
    
    //해당사태에서 빠져 나갈 때 호출되는 메서드
    void Exit();
}
