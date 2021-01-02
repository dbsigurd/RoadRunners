using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RunnerSensor : MonoBehaviour
{
    internal bool SuspendSensors;

    [SerializeField] internal bool
        DetectsSurface,
        DetectsGround,
        DetectsEdge,
        DetectsEdgeL,
        DetectsEdgeR;

    [SerializeField] SimpleSensor _sensorLeft, _sensorRight;

    private void OnTriggerEnter2D(Collider2D collision)
        => UpdateScanResults();

    private void OnTriggerExit2D(Collider2D collision)
        => UpdateScanResults();

    private void UpdateScanResults()
    {
        if (!SuspendSensors)
        {
            DetectsSurface = _sensorLeft.DetectsLayer(0) || _sensorRight.DetectsLayer(0);
            DetectsGround = _sensorLeft.DetectsLayer(31) || _sensorRight.DetectsLayer(31);

            DetectsEdgeR = _sensorLeft.DetectsLayer(0) && !_sensorRight.DetectsLayer(0);
            DetectsEdgeL = !_sensorLeft.DetectsLayer(0) && _sensorRight.DetectsLayer(0);

            DetectsEdge = DetectsEdgeL || DetectsEdgeR;
        }
    }
}
