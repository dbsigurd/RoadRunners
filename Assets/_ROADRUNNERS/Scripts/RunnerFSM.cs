using UnityEngine;

enum RunnerState { Idle, Running, Reaching, Jumping, KnockedOut, WipedOut }

public class RunnerFSM : MonoBehaviour
{
    RunnerState State = RunnerState.Idle;

    internal bool IsIdle        => State == RunnerState.Idle;
    internal bool IsRunning     => State == RunnerState.Running;
    internal bool IsReaching    => State == RunnerState.Reaching;
    internal bool IsJumping     => State == RunnerState.Jumping;
    internal bool IsKnockedOut  => State == RunnerState.KnockedOut;
    internal bool IsWipedOut    => State == RunnerState.WipedOut;


    Animator        _animator;
    PlayerInput     _input;
    SpriteRenderer  _renderer;
    RunnerSensor    _sensor;

    private void Start()
    {
        _animator   = GetComponent<Animator>();
        _input      = GetComponent<PlayerInput>();
        _renderer   = GetComponent<SpriteRenderer>();
        _sensor     = GetComponent<RunnerSensor>();
    }

    private void Update()
    {
        CheckForSpriteFlip();
        UpdateState();
    }

    private void CheckForSpriteFlip()
    {
        if (State != RunnerState.WipedOut && _input.LStickAxisX < 0)
            _renderer.flipX = true;
        else _renderer.flipX = false;
    }

    private void UpdateState()
    {
        var stateLast = State;

        if (_sensor.DetectsGround)
            State = RunnerState.WipedOut;

        else if (!IsWipedOut)
            if (_sensor.DetectsVehicle)
                if (Mathf.Abs(_input.LStickAxisX) > 0.05f)
                    if (_sensor.DetectsEdge)
                        State = RunnerState.Reaching;
                    else State = RunnerState.Running;
                else State = RunnerState.Idle;
            else State = RunnerState.Jumping;
        else if (IsWipedOut && !_sensor.DetectsGround)
            State = RunnerState.KnockedOut;
        else State = RunnerState.WipedOut;

        _animator.SetFloat("Speed", Mathf.Abs(_input.LStickAxisX));

        if (State != stateLast)
            _animator.SetTrigger($"{State}");
    }
}
