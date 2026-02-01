#nullable enable
using UnityEditor;
using UnityEngine;
using UnityUtils;

[RequireComponent(typeof(AudioSource))]
public class Turret : MonoBehaviour
{
    [SerializeField] private LayerMask _disablingLayers;
    [SerializeField] private GameObject? _bulletPrefab;
    [SerializeField] private AudioClip? _shootSound;
    [SerializeField] private AudioSource _audioSource = null!;
    [SerializeField] private float _interval = 2f;
    [SerializeReference] private Transform? _bulletOrigin = null;
    private float _cooldown;
    static private float _lastAudioTimestamp = 0;

    private void Start()
    {
        _cooldown = _interval;
        _audioSource = GetComponent<AudioSource>();
    }

    private void Update()
    {
        _cooldown -= Time.deltaTime;

        if (_cooldown <= 0f)
        {
            if (!_bulletPrefab) return;
            _cooldown += _interval;
            GameObject bullet = GameObject.Instantiate(_bulletPrefab, _bulletOrigin?.position ?? transform.position, Quaternion.LookRotation(transform.forward.With(y: 0)));
            if (Time.time - _lastAudioTimestamp > _audioSource.clip.length)
            {
                _audioSource.Play();
                _lastAudioTimestamp = Time.time;
            }
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (_disablingLayers.Contains(collision.collider.gameObject.layer))
        {
            foreach (MeshRenderer meshRenderer in GetComponentsInChildren<MeshRenderer>())
            {
                meshRenderer.material.SetColor("_OverlayColor", Color.gray4);
            }
            Destroy(this);
            Debug.Log($"{name} was disabled by {collision.collider.name}");
        }
    }
}
#nullable disable
