using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FoodBehaviour : MonoBehaviour
{
    [SerializeField] private GameObject _burstPRefab;

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Fish"))
        {
            var effect = Instantiate(_burstPRefab, this.gameObject.transform.position, Quaternion.identity);
            Destroy(gameObject);
            Destroy(effect, 2.0f);
        }
    }
}
