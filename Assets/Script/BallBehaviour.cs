using System;
using System.Collections;
using System.Collections.Generic;
using Asteroids.SharedSimple;
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
    private List<NetworkBehaviourId> _playerDataNetworkedIds;

    public override void Spawned()
    {
        _rigidbody = GetComponent<Rigidbody>();
        _playerDataNetworkedIds = FindObjectOfType<GameController>().GetPlayerDataNetworkedIds();
    }

    public void ServeBall(Vector3 force)
    {
        _rigidbody.AddForce(force);
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
