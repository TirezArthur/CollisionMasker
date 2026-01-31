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
        if (collision.gameObject.layer == gameObject.layer)
        {
            GameObject effect = Instantiate(_impactEffectPrefab);
            effect.transform.position = gameObject.transform.position;
            Destroy(effect, 2f);
        }

        Destroy(gameObject);
    }
}
