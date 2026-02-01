#nullable enable
using System.Collections;
using Unity.Mathematics;
using UnityEditor;
using UnityEngine;
using UnityUtils;

[RequireComponent(typeof(AudioSource))]
public class Turret : MonoBehaviour
{
    [SerializeField] private LayerMask _disablingLayers;
    [SerializeField] private GameObject? _bulletPrefab;
    [SerializeField] private AudioClip? _shootSound;
    [SerializeField] private AudioClip? _shutdownSound;
    [SerializeField] private AudioClip? _collisionSound;
    [SerializeField] private ParticleSystem? _particleSystem;
    private AudioSource _audioSource = null!;
    [SerializeField] private float _interval = 2f;
    [SerializeReference] private Transform? _bulletOrigin = null;
    private float _cooldown;
    private bool _disabled = false;
    static private float _lastAudioTimestampShoot = 0;
    private float _lastAudioTimestampCollision = 0;

    private void Start()
    {
        _cooldown = _interval;
        _audioSource = GetComponent<AudioSource>();
    }

    private void Update()
    {
        if (_disabled) return;
        _cooldown -= Time.deltaTime;

        if (_cooldown <= 0f)
        {
            if (!_bulletPrefab) return;
            _cooldown += _interval;
            GameObject bullet = GameObject.Instantiate(_bulletPrefab, _bulletOrigin?.position ?? transform.position, Quaternion.LookRotation(transform.forward.With(y: 0)));
            if (Time.time - _lastAudioTimestampShoot > _audioSource.clip.length)
            {
                _audioSource.Play();
                _lastAudioTimestampShoot = Time.time;
            }
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (_disablingLayers.Contains(collision.collider.gameObject.layer))
        {
            // Collision audio
            if (!_collisionSound || Time.time - _lastAudioTimestampCollision > _collisionSound.length) 
            {
                if (_shutdownSound && _audioSource) _audioSource.PlayOneShot(_collisionSound, 0.55f);
                _lastAudioTimestampCollision = Time.time;
            }

            // Shutdown
            if (_disabled) return;
            StartCoroutine(ShutdownSequence());
            Debug.Log($"{name} was disabled by {collision.collider.name}");
        }
    }

    private IEnumerator ShutdownSequence()
    {
        _cooldown = float.PositiveInfinity;
        _disabled = true;

        yield return new WaitForSeconds(0.15f);
        if (_shutdownSound && _audioSource) _audioSource.PlayOneShot(_shutdownSound);
        if (_particleSystem) _particleSystem.Play();

        const float fadeDuration = 0.8f;
        float start = Time.time;
        float end = start + fadeDuration;
        while (Time.time < end)
        {
            float t = Time.time - start;
            float color = math.lerp(1f, 0.4f, math.saturate(t / fadeDuration));
            foreach (MeshRenderer meshRenderer in GetComponentsInChildren<MeshRenderer>())
            {
                meshRenderer.material.SetColor("_OverlayColor", new Color(color, color, color));
            }
            yield return null;
        }
        yield break;
    }
}
#nullable disable
