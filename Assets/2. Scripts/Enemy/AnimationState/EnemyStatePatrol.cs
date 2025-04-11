using UnityEngine;
using UnityEngine.AI;

public class EnemyStatePatrol : IEnemyState
{
    private EnemyController _enemyController;

    public void Enter(EnemyController enemyController)
    {
        _enemyController = enemyController;

        var patrolPoint = FindRandomPatrolPoint();
        if (patrolPoint != _enemyController.transform.position)
        {
            _enemyController.SetState(EnemyState.Idle);
            return;
        }

        if (_enemyController.Agent.enabled == true)
        {
            _enemyController.Agent.isStopped = false;
            _enemyController.Agent.SetDestination(patrolPoint);
        }
        
        _enemyController.EnemyAnimator.SetBool("Patrol", true);
    }

    public void Update()
    {
        //감지 영역에 플레이어가 있는지 확인해서 있으면 트레이스로 전환
        var detectPlayerTransform = _enemyController.DetectPlayerInCircle();
        if (detectPlayerTransform)
        {
            _enemyController.SetState(EnemyState.Trace);
            return;
        }

        //패트롤 목적지에 도착하면 아이들로 전환
        var destinationDistance =
            Vector3.Distance(_enemyController.transform.position, _enemyController.Agent.destination);
        
        if (!_enemyController.Agent.pathPending && destinationDistance < _enemyController.Agent.stoppingDistance)
        {
            _enemyController.SetState(EnemyState.Idle);
            return;
        }
        
    }

    public void Exit()
    {
        _enemyController.EnemyAnimator.SetBool("Patrol", false);
        _enemyController = null;
    }

    private Vector3 FindRandomPatrolPoint()
    {
        Vector3 randomDirection = Random.insideUnitSphere * _enemyController.DetectCircleRadius;
        randomDirection += _enemyController.transform.position;

        NavMeshHit hit;
        if (NavMesh.SamplePosition(randomDirection, out hit, _enemyController.DetectCircleRadius, NavMesh.AllAreas))
        {
            return hit.position;
        }
        else //못찾았다면
        {
            return _enemyController.transform.position;
        }
    }
}