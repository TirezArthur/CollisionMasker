using UnityEngine;
using UnityEngine.InputSystem;
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
    private Rigidbody _rigidbody;
    private CapsuleCollider _collider;
    [SerializeField] private float _movementSpeed;

    void Start()
    {
        if (!_inputAsset) Debug.LogError($"No input asset assigned to {name}");

        _inputMap = _inputAsset.FindActionMap("Player");
        if (_inputMap != null) _movementAction = _inputMap.FindAction("Movement");

        _rigidbody = GetComponent<Rigidbody>();
        _collider = GetComponent<CapsuleCollider>();
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
            direction = Vector3.ProjectOnPlane(direction, hit.normal);

            _rigidbody.AddForce(direction * _movementSpeed, ForceMode.Force);
        }
    }
}
