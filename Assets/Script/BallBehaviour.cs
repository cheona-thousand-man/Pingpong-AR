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
    private GameController _gameController;

    public override void Spawned()
    {
        _rigidbody = GetComponent<Rigidbody>();
        _gameController = FindObjectOfType<GameController>();
    }

    public void ServeBall(Vector3 force)
    {
        _rigidbody.AddForce(force);
    }

    public void FixedUpdateNetworked()
    {
        CheckBallDropOrStopped();
    }

    private void CheckBallDropOrStopped()
    {
        throw new NotImplementedException();
    }

    private void OnCollisionEnter(Collision other)
    {
        if (other.gameObject.CompareTag("Player"))
        {

        }
        else if (other.gameObject.CompareTag("TableUp"))
        {
            
        }
        else if (other.gameObject.CompareTag("TableDown"))
        {
            
        }
        else if (other.gameObject.CompareTag("TableNet"))
        {
            
        }
        else if (other.gameObject.CompareTag("Court"))
        {
            
        }
    }
}
