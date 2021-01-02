using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class RunnerController : MonoBehaviour
{
    Rigidbody2D _rigidbody;
    PlayerInput _input;
    RunnerFSM _runner;
    RunnerSensor _sensor;
    CircleCollider2D _collider;
    CapsuleCollider2D _trigger;

    [SerializeField] float _speed, _jumpForce = 10f;

    internal Vector2 _defaultVelocity = Vector2.zero;
    internal Vector2 RelativeVelocity
        => new Vector2(_defaultVelocity.x, _rigidbody.velocity.y);
    internal Vector2 InputVelocity
        => new Vector2(_defaultVelocity.x + (_input.LStickAxisX * _speed * Time.fixedDeltaTime), _rigidbody.velocity.y);

    private void Start()
    {
        _rigidbody = GetComponent<Rigidbody2D>();
        _input = GetComponent<PlayerInput>();
        _runner = GetComponent<RunnerFSM>();
        _sensor = GetComponent<RunnerSensor>();
        _collider = GetComponent<CircleCollider2D>();
        _trigger = GetComponent<CapsuleCollider2D>();
    }

    private void Update()
    {
        if (_input.AButtonPressed && _runner.State != RunnerState.Jumping)
            _rigidbody.AddForce(Vector2.up * _jumpForce);

        if (_rigidbody.velocity.y > 0)
            _sensor.SuspendSensors = true;
        else _sensor.SuspendSensors = false;

        if (_runner.State == RunnerState.Ragdoll && _input.AButtonPressed)
            SceneManager.LoadScene(0);

    }

    private void FixedUpdate()
    {
        ProcessMovement();
    }

    private void ProcessMovement()
    {

        if (_runner.State == RunnerState.Ragdoll)
            _rigidbody.velocity = Vector2.Lerp(_rigidbody.velocity, Vector2.zero, 0.05f);

        else
        {
            var absThrow = Mathf.Abs(_input.LStickAxisX);

            var WalkingOffEdgeL = _sensor.DetectsEdgeL && _input.LStickAxisX < 0f;
            var WalkingOffEdgeR = _sensor.DetectsEdgeR && _input.LStickAxisX > 0f;

            if (absThrow > 0.05f && (WalkingOffEdgeL || WalkingOffEdgeR))
                    _rigidbody.velocity = RelativeVelocity;
            else _rigidbody.velocity = InputVelocity;
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (_rigidbody.velocity.y > 0)
            Physics2D.IgnoreCollision(_collider, collision.collider, true);
    }

    private void OnCollisionExit2D(Collision2D collision)
    {
        if (_rigidbody.velocity.y > 0)
            Physics2D.IgnoreCollision(_collider, collision.collider, false);
    }

    private void OnCollisionStay2D(Collision2D collision)
    {
        var otherRB = collision.collider.GetComponent<Rigidbody2D>();

        if (otherRB != null)
            _defaultVelocity = otherRB.velocity;
    }
}
