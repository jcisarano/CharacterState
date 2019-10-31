using jcisarano.CharacterStateMachine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Mover : MonoBehaviour
{
    public Vector3 _moveDir = Vector3.up;
    public float _moveDistance = 5.0f;

    public float _speed = 2.0f;

    Vector3 _currentDest;

    List<Vector3> _waypoints;
    int _waypointIndex = 0;

    CharacterStateMachine _stateMachine;

    public float _health = 100;

    private void Awake()
    {

        InitStateMachine();
    }

    protected virtual void InitStateMachine()
    {
        _stateMachine = gameObject.AddComponent<CharacterStateMachine>();
        _stateMachine.Debug = true;
        _stateMachine.SetDelegateForState(CharacterState.PRE_INIT, Init);
        _stateMachine.SetDelegateForState(CharacterState.INIT_MOVE, InitMove);
        _stateMachine.SetDelegateForState(CharacterState.MOVE_TO, MoveTo);
        _stateMachine.SetDelegateForState(CharacterState.MOVE_COMPLETE, OnMoveComplete);
    }

    protected virtual void Init()
    {
        _waypoints = new List<Vector3>();
        _waypoints.Add(transform.position);
        _waypoints.Add(transform.position + (_moveDir.normalized * _moveDistance));

        _stateMachine.SetState(CharacterState.INIT_MOVE);
    }

    protected virtual void MoveTo()
    {
        if ((_currentDest - transform.position).magnitude < 0.5f)
        {

            _stateMachine.SetNextState();
            return;
        }

        transform.position = Vector3.Lerp(transform.position, _currentDest, Time.deltaTime * _speed);
    }

    protected virtual void InitMove()
    {
        ++_waypointIndex;
        if (_waypointIndex >= _waypoints.Count)
        {
            _waypointIndex = 0;
        }
        _currentDest = _waypoints[_waypointIndex];

        _stateMachine.SetNextState();
    }

    protected virtual void OnMoveComplete()
    {
        _stateMachine.SetNextState();
    }

    public virtual void TakeDamage(int amount)
    {
        _health -= amount;

        if (_health <= 0)
        {
            Die();
        }
    }

    protected virtual void Die()
    {
        Destroy(gameObject);
    }
}
