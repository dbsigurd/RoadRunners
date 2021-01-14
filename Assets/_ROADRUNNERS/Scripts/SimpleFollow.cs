using UnityEngine;

//TODO: Implement boolean condition for overrides

public class SimpleFollow : MonoBehaviour
{
    [SerializeField]
    Transform _target;

    [SerializeField]
    bool
        _overrideX,
        _overrideY,
        _overrideZ;

    [SerializeField] [Tooltip("Will lock checked axis to these values.")]
    Vector3 _override;

    [SerializeField] [Tooltip("Adds these values to Target position/overrides")]
    Vector3 _offset;

    private void LateUpdate()
        => UpdatePosition();

    private void OnValidate()
        => UpdatePosition();

    internal void UpdatePosition()
    {
        float x, y, z;

        if (_overrideX) x = _override.x;
        else x = _target.position.x;

        if (_overrideY) y = _override.y;
        else y = _target.position.y;

        if (_overrideZ) z = _override.z;
        else z = _target.position.z;

        transform.position
            = new Vector3(x, y, z) + _offset;
    }
}
