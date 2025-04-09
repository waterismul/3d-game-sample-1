using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;

public enum EnemyState{Idle, Patrol, Trace, Attack, Hit, Dead}//Patrol: 정찰, Trace: 추적 

[RequireComponent(typeof(Animator))]
[RequireComponent(typeof(NavMeshAgent))]

public class EnemyControllerOld : MonoBehaviour
{
    [Header("Enemy")] 
    [SerializeField] private int attackPower = 1;
    [SerializeField] private int maxHealth = 100;
    
    public Animator EnemyAnimator { get; private set; }

    private int _currentHealth;
    private EnemyState _currentState;
    
    private NavMeshAgent _navMeshAgent;

    private void Awake()
    {
        EnemyAnimator = GetComponent<Animator>();
        _navMeshAgent = GetComponent<NavMeshAgent>();
        _navMeshAgent.updatePosition = false;//직접 움직이지 못하게
        _navMeshAgent.updateRotation = false;//직접 회전하지 못하게
    }

    private void Start()
    {
        _currentHealth = maxHealth;
        
        SetState(EnemyState.Idle);
    }

    private void Update()
    {
        switch (_currentState)
        {
            case EnemyState.Idle:
                break;
            case EnemyState.Patrol:
                break;
            case EnemyState.Trace:
                break;
            case EnemyState.Attack:
                break;
            case EnemyState.Hit:
                break;
            case EnemyState.Dead:
                break;
        }
    }

    public void SetState(EnemyState newState)
    {
        switch (newState)
        {
            case EnemyState.Idle:
                break;
            case EnemyState.Patrol:
                break;
            case EnemyState.Trace:
                break;
            case EnemyState.Attack:
                break;
            case EnemyState.Hit:
                break;
            case EnemyState.Dead:
                break;
        }

        switch (_currentState)
        {
            case EnemyState.Idle:
                break;
            case EnemyState.Patrol:
                break;
            case EnemyState.Trace:
                break;
            case EnemyState.Attack:
                break;
            case EnemyState.Hit:
                break;
            case EnemyState.Dead:
                break;
        }
        
        _currentState = newState;
    }

    #region 동작처리

    private void OnAnimatorMove()
    {
        var position = EnemyAnimator.rootPosition;
        
        position.y = _navMeshAgent.nextPosition.y;
        
        _navMeshAgent.nextPosition = position;
        transform.position = position;
    }

    #endregion

    #region 디버깅

    private void OnDrawGizmos()
    {
        
    }

    #endregion
    
}
