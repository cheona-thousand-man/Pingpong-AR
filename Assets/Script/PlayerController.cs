using Fusion;
using UnityEngine;

namespace Asteroids.SharedSimple
{
    public class PlayerController : NetworkBehaviour
    {
        // Game Session AGNOSTIC Settings
        [SerializeField] private float _rotationSpeed = 12.0f;
        [SerializeField] private float _acceleration = 5000.0f;
        [SerializeField] private float _maxSpeed = 1000.0f;

        // Game Session SPECIFIC Settings
        [Networked] private float Acceleration { get; set; }

        // Local Runtime references
        private Rigidbody _rigidbody;

        public bool AcceptInput => Object.IsValid;

        private void Awake()
        {
            // We're controlling the ship using forces, so grab the rigidbody
            _rigidbody = GetComponent<Rigidbody>();
        }

        public override void FixedUpdateNetwork()
        {
            // Handle input if we have StateAuthority over the object (GetInput returns false otherwise)
            if (AcceptInput)
            {
                Debug.Log("AcceptInput is true");
                if (GetInput<PlayerInput>(out var input))
                {
                    Debug.Log("입력이 눌렸다.");
                    Move(input);
                }
                else
                {
                    Debug.Log("GetInput failed to get input");
                }
            }
            else
            {
                Debug.Log("AcceptInput is false");
            }
        }

        // Moves the spaceship RB using the input for the client with InputAuthority over the object
        private void Move(PlayerInput input)
        {
            Transform xform = transform;
            _rigidbody.AddRelativeTorque(
                Mathf.Clamp(input.HorizontalInput, -1, 1) * _rotationSpeed * Runner.DeltaTime * xform.up,
                ForceMode.VelocityChange);

            Acceleration = Mathf.Clamp(input.VerticalInput, -1, 1) * _acceleration * Runner.DeltaTime;
            Vector3 force = xform.forward * Acceleration;
            _rigidbody.AddForce(force);

            if (_rigidbody.velocity.magnitude > _maxSpeed)
                _rigidbody.velocity = _rigidbody.velocity.normalized * _maxSpeed;
        }
    }
}