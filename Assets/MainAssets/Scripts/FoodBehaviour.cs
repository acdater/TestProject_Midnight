using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;

public class FoodBehaviour : MonoBehaviour
{
    [SerializeField] private GameObject _burstPrefab;

    public Action<Vector3> OnSpawnNewFishAt;

    private void OnTriggerEnter(Collider other)
    {
        this.GetComponent<SphereCollider>().enabled = false;

        if (other.gameObject.CompareTag("Fish"))
        {
            var effect = Instantiate(_burstPrefab, this.gameObject.transform.position, Quaternion.identity);

            Collider[] hitColliders = Physics.OverlapSphere(transform.position, 2.0f);
            var fishes = hitColliders.Where(c => c.gameObject.CompareTag("Fish"));

            if (fishes.Count() >= 2)
            {
                var pos = Vector3.Slerp(fishes.First().transform.position, fishes.Last().transform.position, 0.5f);
                Debug.Log("On spawn is called by: " + other.gameObject.name);
                OnSpawnNewFishAt?.Invoke(pos);
            }

            Destroy(gameObject);
            Destroy(effect, 2.0f);
        }
    }
}
