using UnityEngine;

public class VehicleBehaviour : MonoBehaviour
{
    Rigidbody2D _rigidbody;

    [SerializeField] internal LaneSpawner _parentLane;

    [SerializeField] float _maxSpeed;
    [SerializeField] [Range(0f, 1f)] float _acceleration, _braking;

    [SerializeField] internal bool _runnerOnBoard;

    Vector2 TargetVelocity => new Vector2(_maxSpeed, 0f);

    //Vector2 _tailgateVelocity;

    private void Awake()
    {
        _rigidbody = GetComponent<Rigidbody2D>();
    }

    private void Update()
    {
        if (Mathf.Abs(_rigidbody.position.x - _parentLane.transform.position.x) > 25f)
            gameObject.SetActive(false);
    }

    private void FixedUpdate()
    {
        if (_runnerOnBoard)
            _rigidbody.velocity = Vector2.Lerp
                (_rigidbody.velocity, Vector2.zero, _braking * Time.fixedDeltaTime);

        else _rigidbody.velocity = Vector2.Lerp
                (_rigidbody.velocity, TargetVelocity, _acceleration * Time.fixedDeltaTime);
    }

    private void OnEnable()
        => _rigidbody.velocity = TargetVelocity;

    private void OnDisable()
        => _parentLane.RecallVehicle(this);


    private void OnCollisionStay2D(Collision2D collision)
        => _runnerOnBoard = collision.gameObject.CompareTag("Runner");

    private void OnCollisionExit2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Runner"))
            _runnerOnBoard = false;
    }
}
