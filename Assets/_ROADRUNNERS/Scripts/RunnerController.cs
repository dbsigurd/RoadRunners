using UnityEngine;
using UnityEngine.SceneManagement;

public class RunnerController : MonoBehaviour
{
    Rigidbody2D _rigidbody;
    PlayerInput _input;
    RunnerFSM _runner;
    RunnerSensor _sensor;
    PolygonCollider2D _collider;

    [SerializeField]
    float
        _speed = 100f,
        _jumpForce = 100f,
        _hangFactor;

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
        _collider = GetComponent<PolygonCollider2D>();
    }

    private void Update() //TODO: Move Jukebox and Scene logic to GameManager
    {
        if (_input.AButtonPressed && !_runner.IsJumping && !_runner.IsWipedOut)
            _rigidbody.AddForce(Vector2.up * _jumpForce);

        if (_input.WPaddlePressed)
            Jukebox.PlayPrevious();

        if (_input.EPaddlePressed)
            Jukebox.PlayNext();

        if (_input.StartSustained && _input.SelectSustained)
            Application.Quit();

        else if (_input.StartReleased)
            SceneManager.LoadScene(0);

        else if (_input.SelectReleased)
            SceneManager.LoadScene(1);
    }

    private void FixedUpdate()
        => ProcessMovement();


    private void ProcessMovement()
    {
        if (_runner.IsWipedOut)
            _rigidbody.velocity = Vector2.Lerp(_rigidbody.velocity, Vector2.zero, 0.05f);

        else
        {
            var absThrow = Mathf.Abs(_input.LStickAxisX);

            var WalkingOffEdgeL = _sensor.DetectsEdgeL && _input.LStickAxisX < 0f;
            var WalkingOffEdgeR = _sensor.DetectsEdgeR && _input.LStickAxisX > 0f;

            if (absThrow > 0.05f && (WalkingOffEdgeL || WalkingOffEdgeR))
                _rigidbody.velocity = RelativeVelocity;
            else _rigidbody.velocity = InputVelocity;

            if (_runner.IsJumping && _input.AButtonSustained)
            {
                _rigidbody.AddForce(Vector2.up * _jumpForce * _hangFactor);
                _rigidbody.gravityScale += Time.fixedDeltaTime;
            }
            else _rigidbody.gravityScale = 1;
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
        => IgnoreCollisionLayer(collision, 29, true);

    private void OnTriggerExit2D(Collider2D collision)
        => IgnoreCollisionLayer(collision, 29, false);

    private void OnCollisionEnter2D(Collision2D collision)
        => AdjustVelocityIfOnboard(collision);

    private void OnCollisionStay2D(Collision2D collision)
        => AdjustVelocityIfOnboard(collision);

    void IgnoreCollisionLayer(Collider2D collision, int layerMask, bool ignore)
    {
        if (collision.gameObject.layer == layerMask)
            Physics2D.IgnoreCollision(_collider, collision, ignore);
    }

    private void AdjustVelocityIfOnboard(Collision2D collision)
    {
        if (_rigidbody.velocity.y <= 0.05f)
        {
            var otherRB = collision.collider.GetComponent<Rigidbody2D>();

            if (otherRB != null)
                _defaultVelocity = otherRB.velocity;
        }
    }
}
