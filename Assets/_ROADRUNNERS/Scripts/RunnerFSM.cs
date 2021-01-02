using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

enum RunnerState { Idle, Running, Reaching, Jumping, Falling, Ragdoll }

public class RunnerFSM : MonoBehaviour
{
    //[HideInInspector]
    [SerializeField] internal RunnerState State = RunnerState.Idle;

    Animator _animator;
    PlayerInput _input;
    SpriteRenderer _renderer;
    RunnerSensor _sensor;

    private void Start()
    {
        _animator = GetComponent<Animator>();
        _input = GetComponent<PlayerInput>();
        _renderer = GetComponent<SpriteRenderer>();
        _sensor = GetComponent<RunnerSensor>();
    }

    private void Update()
    {
        CheckForSpriteFlip();
        UpdateState();
    }

    private void CheckForSpriteFlip()
    {
        if (_input.LStickAxisX < 0)
            _renderer.flipX = true;
        else _renderer.flipX = false;
    }

    private void UpdateState()
    {
        var stateLast = State;

        if (_sensor.DetectsGround)
            State = RunnerState.Ragdoll;

        if (State != RunnerState.Ragdoll)
            if (_sensor.DetectsSurface)
                if (Mathf.Abs(_input.LStickAxisX) > 0.05f)
                    if (_sensor.DetectsEdge)
                        State = RunnerState.Reaching;
                    else State = RunnerState.Running;
                else State = RunnerState.Idle;
            else State = RunnerState.Jumping;
        else if (State == RunnerState.Ragdoll && !_sensor.DetectsGround)
            State = RunnerState.Falling;
        else State = RunnerState.Ragdoll;
            
        //if (stateLast != RunnerState.Ragdoll && State == RunnerState.Ragdoll)
        //    StartCoroutine(GetUpAfterDelay());

        if (State != stateLast)
            _animator.SetTrigger($"{State}");

        _animator.SetFloat("Speed", Mathf.Abs(_input.LStickAxisX));
    }
}
