using UnityEngine;

public class GachaDiceRotation : MonoBehaviour
{
    [SerializeField] private float _rotationSpeed = 20f;

    void FixedUpdate ()
    {
        transform.localRotation = Quaternion.AngleAxis(_rotationSpeed * Time.deltaTime, Vector3.up) * transform.localRotation;
    }
}
