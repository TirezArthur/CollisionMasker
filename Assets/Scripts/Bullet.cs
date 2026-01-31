using UnityEngine;

public class Bullet : MonoBehaviour
{
    [SerializeField] private float _speed = 30f;
    [SerializeField] private GameObject _impactEffectPrefab;

    private void Start()
    {
        GetComponent<Rigidbody>().linearVelocity = transform.forward * _speed;
    }

    private void OnCollisionEnter(Collision collision)
    {
        // If bullet hits bullet, create impact effect
        if (collision.gameObject.layer == gameObject.layer)
        {
            GameObject effect = Instantiate(_impactEffectPrefab);
            effect.transform.position = gameObject.transform.position;
            Destroy(effect, 2f);
        }

        // Apply explosion force to the collided object if it has a Rigidbody
        Rigidbody rbCollision = collision.gameObject.GetComponent<Rigidbody>();
        if (rbCollision != null)
        {
            rbCollision.AddExplosionForce(5f, collision.transform.position, 5f);
        }

        // Destroy the bullet
        Destroy(gameObject);
    }
}
