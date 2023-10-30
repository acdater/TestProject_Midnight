using Assets.MainAssets.Scripts.Jobs;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.Collections;
using Unity.Jobs;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Jobs;

public class FishesBehavior : MonoBehaviour
{
    [SerializeField]
    private FoodSpawner _foodSpawner;

    [SerializeField]
    private GameObject _target;

    [SerializeField]
    private int _numberOfFishes;

    [SerializeField]
    private GameObject _fishPrefab;

    [SerializeField]
    private float _destinationThreshold;

    [SerializeField]
    private Vector3 _areaSize;

    [SerializeField]
    private float _maxVelocity;

    [SerializeField]
    [Tooltip("X - spread multiplier (than bigger then more distance among agents); Y - agents velocity multiplier; Z - average position weight (than bigger then closer agents are)")]
    private Vector3 _weights;

    private NativeArray<Vector3> _foodPositions;

    private NativeArray<Vector3> _positions;
    private NativeArray<Vector3> _velocities;
    private NativeArray<Vector3> _accelerations;
    private TransformAccessArray _transformAccessArray;

    private float _spawnRate = 1.0f;
    private int _reproductionRate = 1;

    private const float _boundsThreshold = 3.0f;
    private const float _multiplier = 10;

    public int CountOfFishes { get => _numberOfFishes; }

    public float MaxVelocity { set => _maxVelocity = value; }

    public float SpawnRate { set => _spawnRate = value; }

    public int ReproductionRate { set => _reproductionRate = value; }

    public void AddFishAtPosition(Vector3 pos)
    {
        var chanceToInit = Random.Range(0.1f, 1.0f);

        if (chanceToInit > _spawnRate)
            return;

        Debug.Log("Repro count:" + _reproductionRate);

        for(int i=0; i<_reproductionRate; i++)
        {
            var newFish = Instantiate(_fishPrefab, pos, Quaternion.identity);
            newFish.transform.position += Vector3.forward * i;
            _transformAccessArray.Add(newFish.transform);

            var positionsList = _positions.ToList();
            positionsList.Add(newFish.transform.position);
            _positions.Dispose();
            _positions = new NativeArray<Vector3>(positionsList.ToArray(), Allocator.Persistent);

            var velocitiesList = _velocities.ToList();
            velocitiesList.Add(_velocities.Last());
            _velocities.Dispose();
            _velocities = new NativeArray<Vector3>(velocitiesList.ToArray(), Allocator.Persistent);

            var accelerationList = _accelerations.ToList();
            accelerationList.Add(_accelerations.Last());
            _accelerations.Dispose();
            _accelerations = new NativeArray<Vector3>(accelerationList.ToArray(), Allocator.Persistent);

            _numberOfFishes = _transformAccessArray.length;
            newFish.name += _numberOfFishes;
        }
    }

    void Start()
    {
        _positions= new NativeArray<Vector3>(_numberOfFishes, Allocator.Persistent);
        _velocities = new NativeArray<Vector3>(_numberOfFishes, Allocator.Persistent);
        _accelerations = new NativeArray<Vector3>(_numberOfFishes, Allocator.Persistent);

        var transforms = new Transform[_numberOfFishes];

        for(int i=0; i<_numberOfFishes; i++)
        {
            var item = Instantiate(_fishPrefab);
            item.name += i+1;
            transforms[i] = item.transform;
            _velocities[i] = Random.insideUnitSphere;
        }

        _transformAccessArray = new TransformAccessArray(transforms);
        
        _foodSpawner.AreaSize = _areaSize;
        _foodSpawner.Threshold = _boundsThreshold;
        _foodSpawner.fishesBehaviour = this;
    }

    void Update()
    {
        _foodPositions = new NativeArray<Vector3>(_foodSpawner.GetFoodPositions().ToArray(), Allocator.Persistent);

        var boundsJob = new BoundsJob()
        {
            Accelerations = _accelerations,
            Positions = _positions,
            AreaSize = _areaSize,
            Multiplier = _multiplier,
            Threshold = _boundsThreshold
        };

        var accelerationJob = new AccelerationJob()
        {
            Positions = _positions,
            Velocities = _velocities,
            Accelerations = _accelerations,
            DestinationThreshold = _destinationThreshold,
            Weights= _weights,
            FoodPositions = _foodPositions
            //TargetPosition = _target.transform.position
        };

        var moveJob = new MoveJob()
        {
            Positions = _positions,
            Velocities = _velocities,
            Accelerations = _accelerations,
            DeltaTime = Time.deltaTime,
            MaxVelocity = _maxVelocity
        };

        var boundsHandle = boundsJob.Schedule(_numberOfFishes, 0);

        var accelerationHandle = accelerationJob.Schedule(_numberOfFishes, 0, boundsHandle);

        var moveHandle = moveJob.Schedule(_transformAccessArray, accelerationHandle);
        moveHandle.Complete();

        _foodPositions.Dispose();
    }

    private void OnDestroy()
    {
        if (_positions != null)
        {
            _positions.Dispose();
        }

        if (_velocities != null)
        {
            _velocities.Dispose();
        }
        
        if (_accelerations != null)
        {
            _accelerations.Dispose();
        }
        
        if (_transformAccessArray.IsUnityNull() == false)
        {
            _transformAccessArray.Dispose();
        }

        if (_foodPositions!= null)
        {
            _foodPositions.Dispose();
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.DrawWireCube(Vector3.zero, _areaSize);
    }
}
