using System;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Rendering.Universal.Internal;
using UnityUtils;

[RequireComponent(typeof(Rigidbody))]
[RequireComponent(typeof(CapsuleCollider))]
public class PlayerMovement : MonoBehaviour
{
    // Input
    [SerializeReference] private InputActionAsset _inputAsset;
    private InputActionMap _inputMap;
    private InputAction _movementAction;

    // Physics
    private Vector3 _lookDirection;
    private Rigidbody _rigidbody;
    private CapsuleCollider _collider;
    [SerializeField] private float _movementSpeed = 10f;
    [SerializeField] private float _rotationSpeed = 8f;

    private bool _died;
    private bool _reachedEnd;
    public bool Died => _died;
    public bool ReachedEnd => _reachedEnd;

    void Start()
    {
        if (!_inputAsset) Debug.LogError($"No input asset assigned to {name}");

        _inputMap = _inputAsset.FindActionMap("Player");
        if (_inputMap != null) _movementAction = _inputMap.FindAction("Movement");

        _rigidbody = GetComponent<Rigidbody>();
        _collider = GetComponent<CapsuleCollider>();
        _lookDirection = transform.forward;
    }

    public void Reset()
    {
        _died = false;
    }   

    private void FixedUpdate()
    {
        if (_movementAction == null) return;

        Vector2 inputDirection = _movementAction.ReadValue<Vector2>();

        if (!inputDirection.Equals(Vector2.zero)) 
        {
            if (!Physics.SphereCast(transform.position + _collider.center, _collider.radius, Vector3.down, out RaycastHit hit, _collider.height * 0.6f)) return;

            Vector3 direction = new Vector3();
            Transform cameraTransform = Camera.main.transform;

            direction += cameraTransform.forward.With(y: 0).normalized * inputDirection.y;
            direction += cameraTransform.right.With(y: 0).normalized * inputDirection.x;
            direction.Normalize();
            _lookDirection = direction;
            direction = Vector3.ProjectOnPlane(direction, hit.normal);

            _rigidbody.AddForce(direction * _movementSpeed, ForceMode.Force);

        }

        transform.rotation = Quaternion.RotateTowards(transform.rotation, Quaternion.LookRotation(_lookDirection), _rotationSpeed);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.layer == LayerMask.NameToLayer("Ground"))
        {
            Debug.Log($"{name} collided with {other.gameObject.name}");
            _died = true;
        }
        else if (other.gameObject.layer == LayerMask.NameToLayer("Key"))
        {
            Debug.Log($"{name} collided with {other.gameObject.name}");
            Key key = other.gameObject.GetComponent<Key>();
            key.Pickup();
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.layer == LayerMask.NameToLayer("End"))
        {
            Debug.Log($"{name} collided with {collision.gameObject.name}");
            _reachedEnd = true;
        }
        else if (collision.gameObject.layer == LayerMask.NameToLayer("Projectile"))
        {
            Debug.Log($"{name} collided with {collision.gameObject.name}");
            _died = true;
        }
    }
}
