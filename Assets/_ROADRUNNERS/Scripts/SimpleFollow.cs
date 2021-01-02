using UnityEngine;

public class SimpleFollow : MonoBehaviour
{
    [SerializeField]
    Transform _target;

    [SerializeField]
    Vector3
        _override,
        _offset;

    private void LateUpdate()
        => UpdatePosition();

    private void OnValidate()
        => UpdatePosition();

    internal void UpdatePosition()
    {
        float x, y, z;

        if (_override.x != 0) x = _override.x;
        else x = _target.position.x;

        if (_override.y != 0) y = _override.y;
        else y = _target.position.y;

        if (_override.z != 0) z = _override.z;
        else z = _target.position.z;

        transform.position
            = new Vector3(x, y, z) + _offset;
    }
}
