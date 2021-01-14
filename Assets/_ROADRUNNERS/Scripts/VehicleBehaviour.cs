using System.Collections;
using UnityEngine;


// Alright, this one's a little messy. I've made an effort to bake my intent into the code all accross the project, but this script has undergone some major surgeries over it's development.
// The brass tacks of it is that a vehicle will start at and maintain their TargetVelocity unless one of two things occur: The player is onboard, in which case the vehicle slows down at its _braking speed,
// or a vehicle in front of them is travelling slower than they are, in which case they will interpolate their speed to match theirs. Whatever the case, if a vehicle is travelling under a certain percentage of their max speed,
// they accumulate rage, which results in horn-honking and swearing. Rage will hit a ceiling of 85% if just stuck in traffic, but can reach 100% if the player is onboard, at which point the vehicle explodes.
// Rage decreases if the vehicle is moving at least as fast as the rage threshold, and will reset when the vehicle is recalled to the spawner (by driving or falling behind too far away from the "parkade")


public class VehicleBehaviour : MonoBehaviour
{
    #region Serialized & Internal Fields

    [SerializeField] float _maxSpeed;

    [SerializeField] [Range(0f, 1f)]
    float
        _acceleration,
        _braking,
        _roadRage;

    [SerializeField] AudioClip[] _hornClips;

    internal LaneSpawner ParentLane;

    #endregion
    #region Components & Private Fields

    ParticleSystem _rageParticles;
    ParticleSystem.EmissionModule _rageEmission;
    AudioSource _horn;
    SpriteSwapper _visiblePassenger;
    Rigidbody2D _rigidbody;
    CapsuleCollider2D _bumper;
    RaycastHit2D[] _trafficScanner;
    ContactFilter2D _scanFilter;

    readonly float _scanRange = 2.5f;
    float _startupTimer = 0f;
    bool _runnerOnBoard;

    #endregion
    #region Properties

    bool IsRollingBackwards
        => _rigidbody.velocity.x < 0;

    Vector2 FrontBumper
        => new Vector2
            (_bumper.bounds.center.x + _bumper.bounds.extents.x + 0.1f, _bumper.bounds.center.y);

    Vector2 TargetVelocity
        => new Vector2
            (_maxSpeed * Random.Range(0.75f, 1.5f), 0f);

    #endregion
    #region Unity Lifecycle Methods

    private void Awake()
    {
        _rigidbody = GetComponent<Rigidbody2D>();
        _bumper = GetComponent<CapsuleCollider2D>();
        _scanFilter.SetLayerMask(LayerMask.GetMask("Vehicle"));
        _rageParticles = GetComponentInChildren<ParticleSystem>();
        _rageEmission = _rageParticles.emission;
        _horn = GetComponent<AudioSource>();
        _visiblePassenger = GetComponentInChildren<SpriteSwapper>();
    }

    private void Update()
       => ReturnToPoolIfOutOfBounds();

    private void FixedUpdate()
    {
        if (IsRollingBackwards)
            _rigidbody.velocity = Vector2.zero;

        if (_runnerOnBoard)
            _rigidbody.velocity = Vector2.Lerp
                (_rigidbody.velocity, Vector2.zero, _braking * Time.fixedDeltaTime);

        else
        {
            if (DetectsTraffic())
            {
                var brakingFactor = (_scanRange - _trafficScanner[0].distance) / (_scanRange * _scanRange * _scanRange);

                _rigidbody.velocity = Vector2.Lerp
                        (_rigidbody.velocity, _trafficScanner[0].rigidbody.velocity, brakingFactor);
            }
            else
            {
                _startupTimer += Time.fixedDeltaTime;

                if (_startupTimer > 2f)
                    _rigidbody.velocity = Vector2.Lerp
                        (_rigidbody.velocity, TargetVelocity, _acceleration * Time.fixedDeltaTime);
            }
        }
    }

    private void OnEnable()
    {
        if (_visiblePassenger != null)
            _visiblePassenger.SwapSprite();

        _roadRage = 0f;
        _rigidbody.velocity = TargetVelocity;
        StartCoroutine(RoadRageRoutine());
        StartCoroutine(HonkToFeelBetter());
    }

    private void OnDisable()
    {
        StopAllCoroutines();
        ParentLane.RecallVehicle(this);
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Vehicle"))
            _rigidbody.velocity = Vector2.zero;

        if (collision.gameObject.CompareTag("Runner"))
            _rigidbody.velocity *= 0.85f;
    }

    private void OnCollisionStay2D(Collision2D collision)
        => _runnerOnBoard = collision.gameObject.CompareTag("Runner");

    private void OnCollisionExit2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Runner"))
        {
            _runnerOnBoard = false;
            _startupTimer = 0f;
        }
    }

    #if UNITY_EDITOR
    private void OnDrawGizmos()
        => Gizmos.DrawLine
            (
                new Vector3(_bumper.bounds.center.x + _bumper.bounds.extents.x + 0.1f, _bumper.bounds.center.y, 0f),
                new Vector3(_bumper.bounds.center.x + _bumper.bounds.extents.x + _scanRange, _bumper.bounds.center.y, 0f)
            );
    #endif

    #endregion
    #region Private Methods

    private void ReturnToPoolIfOutOfBounds()
    {
        if (Mathf.Abs(_rigidbody.position.x - ParentLane.Parkade.position.x) > 25f)
            gameObject.SetActive(false);
    }
    private bool DetectsTraffic()
    {
        _trafficScanner = new RaycastHit2D[1];

        Physics2D.Raycast(FrontBumper, Vector2.right, _scanFilter, _trafficScanner, _scanRange);
        return _trafficScanner[0].rigidbody != null;
    }


    private IEnumerator RoadRageRoutine()
    {
        _rageEmission.rateOverTime = 0f;
        _rageParticles.Play();

        float rageInterval = 0.25f;

        yield return new WaitForSeconds(Random.Range(0f, rageInterval));

        while (true)
        {
            if (_roadRage > 0.5f)
                _rageEmission.rateOverTime = _roadRage * 5f;
            else _rageEmission.rateOverTime = 0;

            if (_roadRage < 0f) _roadRage = 0f;

            if (_runnerOnBoard)
                _roadRage += rageInterval * 0.12f;
            else if (DetectsTraffic() && _rigidbody.velocity.magnitude < TargetVelocity.magnitude * 0.75f && _roadRage <= 0.85f)
                _roadRage += rageInterval * 0.09f;
            else _roadRage -= rageInterval * 0.06f;

            if (_roadRage >= 1)
            {
                AngerManagement.Explode(transform.position);

                _roadRage = 0f;
                gameObject.SetActive(false);
            }

            yield return new WaitForSeconds(rageInterval);
            continue;
        }
    }

    private IEnumerator HonkToFeelBetter()
    {
        var velocityLast = TargetVelocity;

        var hornInterval = 2.5f;

        yield return new WaitForSeconds(Random.Range(0f, hornInterval));

        while (true)
        {
            if (_rigidbody.velocity.magnitude < TargetVelocity.magnitude * 0.75f && velocityLast.x > _rigidbody.velocity.x && _roadRage > 0.5f)
            {
                _horn.clip = _hornClips[Random.Range(0, _hornClips.Length - 1)];
                _horn.pitch = Random.Range(0.85f, 1.15f);
                _horn.volume = Random.Range(0.85f, 1.15f);
                _horn.panStereo = Random.Range(-1f, 1f);
                _horn.Play();
            }

            velocityLast = _rigidbody.velocity;

            yield return new WaitForSeconds(Random.Range(hornInterval * 0.5f, hornInterval));
        }
    }

    #endregion
}


