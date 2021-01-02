using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Collider2D))]
public class SimpleSensor : MonoBehaviour
{
    [SerializeField] Collider2D _sensor;
    [SerializeField] List<Collider2D> _colliders;

    private void Awake()
    {
        if (_sensor == null)
            _sensor = GetComponent<Collider2D>();
    }

    private void Start()
        => _sensor.isTrigger = true;

    private void OnTriggerEnter2D(Collider2D collision)
        => _colliders.Add(collision);


    private void OnTriggerExit2D(Collider2D collision)
        => _colliders.Remove(collision);

    internal bool DetectsLayer(int layerInt)
    {
        foreach (var collider in _colliders)
            if (collider.gameObject.layer == layerInt)
                return true;
            else continue;

        return false;
    }
}