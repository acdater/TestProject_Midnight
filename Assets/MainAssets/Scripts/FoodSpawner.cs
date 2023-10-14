using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FoodSpawner : MonoBehaviour
{
    [SerializeField]
    private GameObject _foodPrefab; // in

    [SerializeField]
    private List<Vector3> _foodPositions; // out

    [SerializeField]
    private Vector3 _areaSize; // in

    [SerializeField]
    private float _threshold; // in

    [SerializeField]
    private float _minGenerateInterval;

    [SerializeField]
    private float _maxGenerateInterval;

    [SerializeField]
    private float _minGenerateCount;

    [SerializeField]
    private float _maxGenerateCount;

    private Vector3 Size => _areaSize * 0.5f;

    public IEnumerable<Vector3> GetFoodPositions() => _foodPositions;

    public Vector3 AreaSize { set => _areaSize = value; }
    public float Threshold { set => _threshold = value; }

    private Coroutine _generateCoroutine;

    public void GenerateFood(int count)
    {
        for(int i=0; i<count; i++)
        {
            GenerateOneFoodPiece();
        }
    }


    void Update()
    {
        if (_generateCoroutine == null)
        {
            _generateCoroutine = StartCoroutine(nameof(GenerateFoodForRandomTime));
        }
    }

    private void GenerateOneFoodPiece()
    {
        var position = GeneratePosition();
        Instantiate(_foodPrefab, position, Quaternion.identity);
        _foodPositions.Add(position);
    }

    private Vector3 GeneratePosition()
    {
        var posX = Random.Range(-Size.x + _threshold, Size.x - _threshold);
        var posY = Random.Range(-Size.y + _threshold, Size.y - _threshold);
        var posZ = Random.Range(-Size.z + _threshold, Size.z - _threshold);

        return new Vector3(posX, posY, posZ);
    }

    private IEnumerator GenerateFoodForRandomTime()
    {
        var interval = Random.Range(_minGenerateInterval, _maxGenerateInterval);
        var count = Random.Range(_minGenerateCount, _maxGenerateCount);

        for(int i=0; i<count; i++)
        {
            GenerateOneFoodPiece();
        }

        yield return new WaitForSeconds(interval);
        _generateCoroutine = null;
    }
}
