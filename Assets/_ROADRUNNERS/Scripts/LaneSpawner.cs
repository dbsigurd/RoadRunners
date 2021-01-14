using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LaneSpawner : MonoBehaviour
{
    //SpawnPoints follow the player via SimpleFollow
    [SerializeField] internal Transform WestSpawnPoint, EastSpawnPoint, Parkade;

    [SerializeField] List<VehicleBehaviour> CarPool = new List<VehicleBehaviour>();
    [SerializeField] float _spawnIntervalMin, _spawnIntervalMax;
    [SerializeField] int _vehiclesInLaneMax;
    [SerializeField] ContactFilter2D _scanFilter;

    bool _spawnToggle;
    int _startingPoolCount;

    private void Start()
    {
        foreach (var vehicle in CarPool)
        {
            vehicle.ParentLane = this;
            vehicle.transform.position = Parkade.position;
        }
        _startingPoolCount = CarPool.Count;

        StartCoroutine(DispatchRoutine());
    }

    private IEnumerator DispatchRoutine()
    {
        var dispatchInterval = 0.25f;

        yield return new WaitForSeconds(Random.Range(0, dispatchInterval));

        while (true)
        {
            if (_startingPoolCount - CarPool.Count < _vehiclesInLaneMax)
            {
                var vehicle = CarPool[Random.Range(1, CarPool.Count - 1)];

                if (_spawnToggle)
                {
                    if (CheckForCongestionAtSpawn(EastSpawnPoint.position))
                        DispatchVehicle(vehicle, EastSpawnPoint.position);
                }
                else if (CheckForCongestionAtSpawn(WestSpawnPoint.position))
                    DispatchVehicle(vehicle, WestSpawnPoint.position);
            }
            _spawnToggle = !_spawnToggle;

            yield return new WaitForSeconds(dispatchInterval);
        }
    }

    private bool CheckForCongestionAtSpawn(Vector3 spawnPoint)
    {
        var trafficScanner = new RaycastHit2D[1];

        return Physics2D.Raycast(spawnPoint + new Vector3(0f, 10f, 0f), Vector2.left, _scanFilter, trafficScanner, 2.5f) == 0
            && Physics2D.Raycast(spawnPoint + new Vector3(0f, 10f, 0f), Vector2.right, _scanFilter, trafficScanner, 2.5f) == 0;
    }

    private void DispatchVehicle(VehicleBehaviour vehicle, Vector3 spawnPoint)
    {
        vehicle.transform.position = spawnPoint;
        vehicle.gameObject.SetActive(true);
        CarPool.Remove(vehicle);
    }

    internal void RecallVehicle(VehicleBehaviour vehicle)
    {
        CarPool.Add(vehicle);
        vehicle.transform.position = Parkade.position;
    }
}
