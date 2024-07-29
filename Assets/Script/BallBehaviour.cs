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

    private Rigidbody _rigidbody;

    private void Awake()
    {
        _rigidbody = GetComponent<Rigidbody>();
    }
    public void ServeBall(Vector3 force)
    {
        _rigidbody.AddForce(force);
    }
}
