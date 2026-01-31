#nullable enable
using UnityEditor;
using UnityEngine;
using UnityUtils;

public class Turret : MonoBehaviour
{
    [SerializeField] private GameObject _bulletPrefab;
    [SerializeField] private float _interval = 2f;
    [SerializeReference] private Transform? _bulletOrigin = null;
    private float _cooldown;

    private void Start()
    {
        _cooldown = _interval;
    }

    private void Update()
    {
        _cooldown -= Time.deltaTime;

        if (_cooldown <= 0f)
        {
            _cooldown += _interval;
            GameObject bullet = GameObject.Instantiate(_bulletPrefab, _bulletOrigin?.position ?? transform.position, Quaternion.LookRotation(transform.forward.With(y: 0)));
        }
    }
}
#nullable disable
