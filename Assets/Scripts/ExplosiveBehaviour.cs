using UnityEngine;

public class ExplosiveBehaviour : MonoBehaviour
{
    [SerializeField] private LayerMask m_TargetMask;
    [SerializeField] private ParticleSystem m_ExplosionEffect;
    [SerializeField] private AudioClip m_ExplosionSound;

    private void Start()
    {
        if (m_ExplosionEffect == null)
        {
            Debug.LogWarning("Explosion effect not assigned in ExplosiveBehaviour.");
            return;
        }

        m_ExplosionEffect.Stop();
    }

    private void OnTriggerEnter(Collider other)
    {
        if ((m_TargetMask.value & (1 << other.gameObject.layer)) > 0)
        {
            // Add explosion effect here
            Explode(other);
        }
    }

    private void Explode(Collider other)
    {
        m_ExplosionEffect.Play();

        // Play sound
        AudioSource.PlayClipAtPoint(m_ExplosionSound, transform.position);

        other.gameObject.GetComponent<Rigidbody>()?.AddExplosionForce(30f, transform.position, 5f);

        Destroy(gameObject, 1f);
    }
}
