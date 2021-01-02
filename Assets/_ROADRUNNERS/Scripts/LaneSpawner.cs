using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LaneSpawner : MonoBehaviour
{
    [SerializeField] Transform WestSpawnPoint, EastSpawnPoint;

    [SerializeField] float _spawnIntervalMin, _spawnIntervalMax;

    [SerializeField] int _vehiclesInLaneMax;

    [SerializeField] List<VehicleBehaviour> CarPool = new List<VehicleBehaviour>();

    int _vehiclesDispatched;
    int _startingVehicleCount;

    private void Start()
    {
        foreach (var vehicle in CarPool)
            vehicle._parentLane = this;

        _startingVehicleCount = CarPool.Count;

        StartCoroutine(DispatchVehicles());
    }

    private IEnumerator DispatchVehicles()
    {
        while (true)
        {
            if (_startingVehicleCount - CarPool.Count < _vehiclesInLaneMax)
            {
                var vehicle = CarPool[Random.Range(1, CarPool.Count-1)];

                if (_vehiclesDispatched % 2 == 0)
                    vehicle.transform.position = EastSpawnPoint.position;
                else vehicle.transform.position = WestSpawnPoint.position;

                vehicle.gameObject.SetActive(true);
                CarPool.Remove(vehicle);

                _vehiclesDispatched++;

                yield return new WaitForSeconds
                    (Random.Range(_spawnIntervalMin, _spawnIntervalMax) * 0.5f);
            }
            else yield return null;
        }
    }

    internal void RecallVehicle(VehicleBehaviour vehicle)
    {
        CarPool.Add(vehicle);
    }
}
