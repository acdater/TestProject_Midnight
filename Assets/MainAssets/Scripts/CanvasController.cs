using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class CanvasController : MonoBehaviour
{
    [SerializeField]
    private FishesBehavior _fishesBehaviour;

    [SerializeField]
    private TextMeshProUGUI _countOfItemsField;

    [SerializeField]
    private Slider _maxVelocityOfAgentsSlider;

    [SerializeField]
    private Slider _spawnRateSlider;

    [SerializeField]
    private Slider _reproductionRateSlider;

    [SerializeField] TextMeshProUGUI _speedValueTMP;
    [SerializeField] TextMeshProUGUI _spawnRateValueTMP;
    [SerializeField] TextMeshProUGUI _reproducitonRateValueTMP;

    // Start is called before the first frame update
    void Start()
    {
        SetTMPValues();
    }

    // Update is called once per frame
    void Update()
    {
        if (_fishesBehaviour == null)
            return;

        _countOfItemsField.text = _fishesBehaviour.CountOfFishes.ToString();
    }

    private void SetTMPValues()
    {
        _speedValueTMP.text = string.Format("{0:N2}", _maxVelocityOfAgentsSlider.value);
        _spawnRateValueTMP.text = string.Format("{0:N2}", _spawnRateSlider.value);
        _reproducitonRateValueTMP.text = ((int)_reproductionRateSlider.value).ToString();
    }

    public void UpdateMaxVelocity()
    {
        _fishesBehaviour.MaxVelocity = _maxVelocityOfAgentsSlider.value;
        SetTMPValues();
    }

    public void UpdateSpawnRateValue()
    {
        _fishesBehaviour.SpawnRate = _spawnRateSlider.value;
        SetTMPValues();
    }

    public void UpdateReproductionRateValue()
    {
        _fishesBehaviour.ReproductionRate = (int)_reproductionRateSlider.value;
        SetTMPValues();
    }
}
