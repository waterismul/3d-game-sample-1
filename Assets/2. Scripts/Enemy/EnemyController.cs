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

    public Renderer EnemyRenderer => enemyRenderer;
    [SerializeField] private Renderer enemyRenderer;//맞으면 빨갛게

    [SerializeField] private HPBarController hpBarController;

    public float MaxPatrolWaitTime => maxPatrolWaitTime;
    public float MaxDetectSightAngle => maxDetectSightAngle;
    public float DetectCircleRadius => detectCircleRadius;
    public float MaxAttackDistance => maxAttackDistance;
    public LayerMask TargetLayerMask => targetLayerMask;

    [Header("Ragdoll")]
    [SerializeField] private Collider[] ragdollColliders;
    [SerializeField] private Rigidbody[] ragdollRigdbodies;
    [SerializeField] private CharacterJoint[] ragdollJoints;
    
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

    private Collider _collider;
    private Rigidbody _rigidbody;

    private void Awake()
    {
        EnemyAnimator = GetComponent<Animator>();
        Agent = GetComponent<NavMeshAgent>();
        Agent.updateRotation = true;
        Agent.updatePosition = false;
        
        _collider = GetComponent<Collider>();
        _rigidbody = GetComponent<Rigidbody>();
    }

    private void Start()
    {
        //Ragdoll 비활성화
        SetRagdollEnabled(false);
        
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
        hpBarController.SetHP(_currentHealth/(float)maxHealth);
        
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

    private void OnCollisionEnter(Collision other)//몬스터 죽고 땅에 닿으면 서서히 사라지게
    {
        if (other.gameObject.CompareTag("Ground"))
        {
            SetRagdollEnabled(true);
            StartCoroutine(Dissovle());
        }
    }

    IEnumerator Dissovle()
    {
        var propertyBlock = new MaterialPropertyBlock();
        enemyRenderer.GetPropertyBlock(propertyBlock);
        var value = 0f;
        while (value < 1f)
        {
            value += Time.deltaTime;
            propertyBlock.SetFloat("_Cutoff", value);
            enemyRenderer.SetPropertyBlock(propertyBlock);
            yield return null;
        }
    }

    #region Ragdoll 관련

    private void SetRagdollEnabled(bool isEnabled)
    {
        foreach (var ragdollCollider in ragdollColliders)
        {
            ragdollCollider.enabled = isEnabled;
        }

        foreach (var ragollRigdbody in ragdollRigdbodies)
        {
            ragollRigdbody.detectCollisions = isEnabled;
            ragollRigdbody.isKinematic = !isEnabled;
        }
        
        EnemyAnimator.enabled = !isEnabled;//흐음
        
        _collider.enabled = !isEnabled;
        _rigidbody.detectCollisions = !isEnabled;

        EnemyAnimator.Rebind();
        EnemyAnimator.Update(0f);
    }

    #endregion

    #region Hit 관련

    public void SetHit(PlayerController playerController)
    {
        var attackPower = playerController.AttackPower - defensePower;
        _currentHealth -= attackPower;
        
        hpBarController.SetHP(_currentHealth/(float)maxHealth);

        if (_currentHealth <= 0)
        {
            //Dead 처리
            hpBarController.gameObject.SetActive(false);
            
            SetState(EnemyState.Dead);

            Agent.enabled = false;
            
            _rigidbody.isKinematic = false;
            _rigidbody.useGravity = true;
            _rigidbody.constraints = RigidbodyConstraints.None;

            var direction = transform.position - playerController.transform.position;
            direction.y = 1f;
            direction = direction.normalized;
            
            var force = direction * 20f;

            _rigidbody.AddForce(force, ForceMode.Impulse);

            _collider.isTrigger = false;
        }
        else
        {
            _enemyStateHit.SetAttacker(playerController);
            SetState(EnemyState.Hit);
        }
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
