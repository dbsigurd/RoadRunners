using UnityEngine;
using System.Collections.Generic;

public class RunnerSensor : MonoBehaviour
{
    [SerializeField] Transform _sensorL, _sensorR;
    [SerializeField] ContactFilter2D _filter;

    internal bool DetectsVehicle
        => DetectLayer(29);
    internal bool DetectsGround
        => DetectLayer(31);

    internal bool DetectsEdgeL
        => DetectEdge(_sensorL.position, Vector2.down, 0.01f);
    internal bool DetectsEdgeR
        => DetectEdge(_sensorR.position, Vector2.down, 0.01f);
    internal bool DetectsEdge 
        => DetectsEdgeL || DetectsEdgeR;

    List<Collider2D> _colliders = new List<Collider2D>();


    private void OnCollisionEnter2D(Collision2D collision)
        => _colliders.Add(collision.collider);

    private void OnCollisionExit2D(Collision2D collision)
        => _colliders.Remove(collision.collider);

    private bool DetectLayer(int layer)
    {
        if (_colliders.Count == 0)
            return false;

        foreach (var collider in _colliders)
            if (collider.gameObject.layer == layer)
                return true;
            else continue;

        return false;
    }

    private bool DetectEdge(Vector3 position, Vector2 direction, float range)
    {
        var hits = new RaycastHit2D[1];

        Physics2D.Raycast(position, direction, _filter, hits, range);

        return DetectsVehicle && hits[0].collider == null;
    }
}
