using System;
using System.Collections;
using System.Collections.Generic;
using Fusion;
using UnityEngine;

public class BallBehaviour : NetworkBehaviour
{
    [Networked] public string Owner {get; set;}
    [Networked] public bool IsServed {get; set;}
    [Networked] public bool IsBounced {get; set;}
    [Networked] public Vector3 BallState {get; set;}
    [Networked] public Quaternion Rotation {get; set;}
    private bool _hasHitPlane = false;


    private Rigidbody _rigidbody;

    private void Awake()
    {
        _rigidbody = GetComponent<Rigidbody>();
    }

    public override void FixedUpdateNetwork()
    {
        // If the bullet has not hit an asteroid, moves forward.
        if (HasHitPlane() == true)
        {
            _hasHitPlane = false;
            Runner.Despawn(Object);
            return;
        }
    }

    public void ServeBall(Vector3 force)
    {
        _rigidbody.AddForce(-force);
    }

    private void OnCollisionEnter(Collision collision)
    {
        // Check if the collided object is the plane
        if (collision.gameObject.CompareTag("plane"))
        {
            Debug.Log("부딪힘");
            _hasHitPlane = true;

            if (Runner.TryGetPlayerObject(Object.InputAuthority, out var playerNetworkObject))
            {
                playerNetworkObject.GetComponent<PlayerDataNetworked>().AddToScore(10); // Assuming 10 points for hitting the plane
            }

            Runner.Despawn(Object); // Optionally despawn the bullet
        }


    }

    public bool HasHitPlane()
    {
        return _hasHitPlane;
    }

}
