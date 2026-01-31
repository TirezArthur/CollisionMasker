using UnityEngine;

public class Bullet : MonoBehaviour
{
    [SerializeField] private float _speed = 30f;

    private void Start()
    {
        GetComponent<Rigidbody>().linearVelocity = transform.forward * _speed;
    }

    private void OnCollisionEnter(Collision collision)
    {
        Destroy(gameObject);
    }
}
