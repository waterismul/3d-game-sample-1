using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IObserver<T>
{
    public void OnNext(T value);//특정 이벤트가 발생했을 때 호출
    public void OnError(Exception error);//옵저버가 오류가 났을때 호출
    public void OnCompleted();//이벤트 완료됐을 시
}
