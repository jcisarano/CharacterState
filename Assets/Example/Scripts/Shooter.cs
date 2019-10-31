using jcisarano.CharacterStateMachine;
using jcisarano.laser.standard;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Shooter : MonoBehaviour
{
    public float _rotationSpeed = 1.0f;
    public GameObject _turretHead;

    protected float _timeRemainingOnTarget = 0.0f;
    protected GameObject _currentTarget = null;

    protected bool _fireLaser = true;
    public float _willFireAngle = 20.0f;
    public bool _onlyFireWhenNearTarget = true;

    public GameObject _laserStart;
    public float _laserRange = 10.0f;
    protected LineRenderer _laserLineRenderer;

    public WeaponBase _weapon;

    CharacterStateMachine _stateMachine;

    private void Awake()
    {
        InitStateMachine();
    }

    protected virtual void InitStateMachine()
    {
        _stateMachine = gameObject.AddComponent<CharacterStateMachine>();
        _stateMachine.Debug = true;
        _stateMachine.SetDelegateForState(CharacterState.PRE_INIT, Init);
        _stateMachine.SetDelegateForState(CharacterState.SEARCHING_FOR_TARGET, FindNewTarget);
        _stateMachine.SetDelegateForState(CharacterState.INIT_ATTACK, InitAttack);
        _stateMachine.SetDelegateForState(CharacterState.ATTACKING, DoAttack);
        _stateMachine.SetDelegateForState(CharacterState.ATTACK_COMPLETE, OnAttackComplete);
    }

    protected virtual void Init()
    {
        _laserLineRenderer = GetComponentInChildren<LineRenderer>();

        _stateMachine.SetState(CharacterState.SEARCHING_FOR_TARGET);
    }

    protected virtual void FindNewTarget()
    {
        _currentTarget = FindTarget(_currentTarget);
        _stateMachine.SetNextState();
    }

    protected virtual void InitAttack()
    {
        ResetTargetCooldown();
        _stateMachine.SetNextState();
    }

    protected virtual void DoAttack()
    {
        //switch targets every few seconds
        _timeRemainingOnTarget -= Time.deltaTime;
        if (_timeRemainingOnTarget <= 0.0f || _currentTarget == null)
        {
            _fireLaser = false;
            SetFireState(_fireLaser);
            _stateMachine.SetNextState();
            return;
        }

        Quaternion targetRotation = RotateTowardsTarget(_currentTarget);

        if (_onlyFireWhenNearTarget)
        {
            _fireLaser = IsAlmostOnTarget(_turretHead.transform.rotation, targetRotation);
        }

        SetFireState(_fireLaser);
    }

    protected virtual void OnAttackComplete()
    {
        if (!_onlyFireWhenNearTarget)
            _fireLaser = false;

        _stateMachine.SetNextState();
    }

    protected virtual Quaternion RotateTowardsTarget(GameObject target)
    {
        //Quaternion targetRotation = Quaternion.LookRotation(target.transform.position - _turretHead.transform.position, Vector3.up);
        //targetRotation = Quaternion.Inverse(_turretHead.transform.rotation) * targetRotation;

        Quaternion targetRotation = GetRotationToTarget(_turretHead.transform.position, target.transform.position, transform.up);
        _turretHead.transform.rotation = Quaternion.Slerp(_turretHead.transform.rotation, targetRotation, Time.deltaTime * _rotationSpeed);

        //Vector3 euler = _turretHead.transform.eulerAngles;
        //euler.x = Mathf.Clamp(euler.x, -89.0f, 1.0f);
        //_turretHead.transform.eulerAngles = euler;

        return targetRotation;
    }

    protected virtual Quaternion GetRotationToTarget(Vector3 myPos, Vector3 targetPos, Vector3 up)
    {
        Vector3 direction = GetDirectionToTarget(myPos, targetPos);
        return Quaternion.LookRotation(direction, up);
    }

    protected virtual Vector3 GetDirectionToTarget(Vector3 myPos, Vector3 targetPos)
    {
        return (targetPos - myPos).normalized;
    }

    protected virtual bool IsAlmostOnTarget(Quaternion turretRotation, Quaternion rotationToTarget)
    {
        return Quaternion.Angle(turretRotation, rotationToTarget) < _willFireAngle;
    }

    protected virtual void ResetTargetCooldown()
    {
        _timeRemainingOnTarget = 2.0f + Random.Range(0, 2.0f);
    }

    protected virtual GameObject FindTarget(GameObject currentTarget)
    {
        Mover[] targets = FindObjectsOfType<Mover>();

        if (targets.Length > 0)
        {
            Mover nextTarget = targets[Random.Range(0, targets.Length)];

            //if nextTarget == currentTarget try again?

            return nextTarget.gameObject;
        }

        return null;
    }

    void SetFireState(bool shouldFire)
    {
        if (_weapon != null)
        {
            _weapon.SetFireStatus(shouldFire);
        }
    }
}
