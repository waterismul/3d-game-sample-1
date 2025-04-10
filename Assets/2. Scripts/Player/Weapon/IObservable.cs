using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IObservable<T>
{
    public void Subscribe(IObserver<T> observer);//휘둘러 질수도, 부딪힐수도 있는 뭔가의 상황, 이벤트가 발생했을 때 궁금한 사람이 있다면 참조값 전달해줘 등록
    public void Unsubscribe(IObserver<T> observer);//등록 해제
    public void Notify(T value);//중요한 이벤트가 발생했을 시 이벤트 발생 알림
}
