using System.Collections;
using System.Collections.Generic;
using JetBrains.Annotations;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UIElements;

public enum EnemyState{None, Idle, Patrol, Trace, Attack, Hit, Dead}

[RequireComponent(typeof(NavMeshAgent))]
[RequireComponent(typeof(Animator))]
public class EnemyController : MonoBehaviour
{
    [Header("Basic Info")] 
    [SerializeField] private int maxHealth = 100;
    [SerializeField] private int attackPower = 10;
    [SerializeField] private int defensePower = 5;

    [Header("AI")]
    [SerializeField] private float detectCircleRadius = 10f;
    [SerializeField] private LayerMask targetLayerMask;
    [SerializeField] private float maxDetectSightAngle = 30f;
    [SerializeField] private float maxPatrolWaitTime = 3f;
    [SerializeField] private float maxAttackDistance = 0.5f;//최소 공격 거리

    public float MaxPatrolWaitTime => maxPatrolWaitTime;
    public float MaxDetectSightAngle => maxDetectSightAngle;
    public float DetectCircleRadius => detectCircleRadius;
    public float MaxAttackDistance => maxAttackDistance;
    public LayerMask TargetLayerMask => targetLayerMask;

    
    
    //--상태 변수--
    private EnemyStateIdle _enemyStateIdle;
    private EnemyStateAttack _enemyStateAttack;
    private EnemyStatePatrol _enemyStatePatrol;
    private EnemyStateDead _enemyStateDead;
    private EnemyStateTrace _enemyStateTrace;
    private EnemyStateHit _enemyStateHit;
    
    public EnemyState CurrentState { get; private set; }
    private Dictionary<EnemyState, IEnemyState> _enemyStates;
    
    //--컴포넌트--
    public Animator EnemyAnimator{ get; private set; }
    public NavMeshAgent Agent{ get; private set; }
    
    //--일반 멤버 변수
    private int _currentHealth;

    private void Awake()
    {
        EnemyAnimator = GetComponent<Animator>();
        Agent = GetComponent<NavMeshAgent>();
        Agent.updateRotation = true;
        Agent.updatePosition = false;
    }

    private void Start()
    {
        //상태 객체 생성
        _enemyStateIdle = new EnemyStateIdle();
        _enemyStatePatrol = new EnemyStatePatrol();
        _enemyStateAttack = new EnemyStateAttack();
        _enemyStateDead = new EnemyStateDead();
        _enemyStateHit = new EnemyStateHit();
        _enemyStateTrace = new EnemyStateTrace();

        _enemyStates = new Dictionary<EnemyState, IEnemyState>()
        {
            { EnemyState.Idle , _enemyStateIdle},
            { EnemyState.Patrol , _enemyStatePatrol},
            { EnemyState.Trace , _enemyStateTrace},
            { EnemyState.Hit , _enemyStateHit},
            { EnemyState.Attack , _enemyStateAttack},
            { EnemyState.Dead , _enemyStateDead}

        };
        
        //HP 초기화
        _currentHealth = maxHealth;
        
        //상태 초기화
        SetState(EnemyState.Idle);
    }

    private void Update()
    {
        if (CurrentState != EnemyState.None)
        {
            _enemyStates[CurrentState].Update();
        }
    }

    public void SetState(EnemyState newState)
    {
        if (CurrentState != EnemyState.None)
        {
            _enemyStates[CurrentState].Exit();
        }

        CurrentState = newState;
        _enemyStates[CurrentState].Enter(this);
    }

    #region Hit 관련

    private void OnTriggerEnter(Collider other)
    {
        Debug.Log("## Enemy OnTriggerEnter");
    }

    #endregion

    #region 이동 관련

    private void OnAnimatorMove()
    {
        Vector3 position = EnemyAnimator.rootPosition;
        
        position.y = Agent.nextPosition.y;
        
        Agent.nextPosition = position;
        transform.position = position;
        
    }

    public void PlayStep()
    {
        
    }

    public void Grunt()
    {
        
    }
    
    public void AttackBegin()
    {
        
    }

    public void AttackEnd()
    {
        
    }

    #endregion

    #region Player 감지관련
     public Transform DetectPlayerInCircle()
     {
         var hitColliders = Physics.OverlapSphere(transform.position, detectCircleRadius, targetLayerMask);//저 반경안에 있는 것들을 hitColliders에 저장
         if (hitColliders.Length > 0)//적을 한개만 찾으면 바로 위치 리턴
         {
             return hitColliders[0].transform;
         }
         else
         {
             return null;
         }
         
    }

    #endregion
    
    #region 디버깅

     private void OnDrawGizmos()
     {
         //Circle 감지 범위
         Gizmos.color = Color.yellow;
         Gizmos.DrawWireSphere(transform.position, detectCircleRadius);
         
         //시야각
         Gizmos.color = Color.red;
         Vector3 rightDirection = Quaternion.Euler(0, maxDetectSightAngle, 0) * transform.forward;
         Vector3 leftDirection = Quaternion.Euler(0, -maxDetectSightAngle, 0) * transform.forward;
         Gizmos.DrawRay(transform.position, rightDirection*detectCircleRadius);
         Gizmos.DrawRay(transform.position, leftDirection*detectCircleRadius);
         Gizmos.DrawRay(transform.position, transform.forward *detectCircleRadius);
         
         //Agent 목적지 시각화
         if(Agent!= null && Agent.hasPath)
         {
             Gizmos.color = Color.green;
             Gizmos.DrawSphere(Agent.destination, 0.5f);
             Gizmos.DrawLine(Agent.destination, Agent.destination);
         }
     }
     
     

     #endregion
    
}
