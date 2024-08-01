using System;
using System.Collections;
using System.Collections.Generic;
using Fusion;
using UnityEngine;

public class BallBehaviour : NetworkBehaviour
{
    [Networked] public PlayerRef Owner {get; set;}
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
        if (CheckBallStopped())
        {
            AddScoreToOwner();
            StartCoroutine(DroppedBallDespawn());
        }
    }

    private void OnCollisionEnter(Collision other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            // 충돌한 객체에서 NetworkObject 가져오기
            var networkObject = other.gameObject.GetComponent<NetworkObject>();
            if (networkObject != null)
            {
                // NetworkObject에서 PlayerDataNetworked NetworkBehaviour를 가져오기
                var playerDataNetworked = networkObject.GetComponent<PlayerDataNetworked>();
                if (playerDataNetworked != null)
                {
                    PlayerRef playerRef = NetworkUtilities.GetPlayerRefFromNetworkBehaviourId(Runner, playerDataNetworked.Id);
                    SetOwner(playerRef);
                }
            }
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
            AddScoreToOwner();
            StartCoroutine(DroppedBallDespawn());
        }
    }

    private bool CheckBallStopped()
    {
        return _rigidbody.velocity == Vector3.zero && _rigidbody.angularVelocity == Vector3.zero;
    }

    public void SetOwner(PlayerRef playerRef)
    {
        Owner = playerRef;
    } 
    
    public void AddScoreToOwner()
    {
        // Owner가 Player1일 경우
        if (Owner.Equals(NetworkUtilities.GetPlayerRefFromNetworkBehaviourId(Runner, _gameController.Player1)))
        {
            // NetworkBehaviourId에서 NetworkBehaviour를 찾아 점수 +1
            Runner.TryFindBehaviour<PlayerDataNetworked> (_gameController.Player1, out var playerDataNetworked);
            playerDataNetworked.AddToScore();

            // 공을 놓친 플레이어에게 서브 권한 전달
            ServePlayerChange(_gameController.Player2);
        }
        // Owner가 Player2일 경우
        else if (Owner.Equals(NetworkUtilities.GetPlayerRefFromNetworkBehaviourId(Runner, _gameController.Player2)))
        {
             // NetworkBehaviourId에서 NetworkBehaviour를 찾아 점수 +1
            Runner.TryFindBehaviour<PlayerDataNetworked> (_gameController.Player1, out var playerDataNetworked);
            playerDataNetworked.AddToScore();

            // 공을 놓친 플레이어에게 서브 권한 전달
            ServePlayerChange(_gameController.Player1);
        }
    }

    public void ServePlayerChange(NetworkBehaviourId nextServePlayer)
    {
        var playerRef = NetworkUtilities.GetPlayerRefFromNetworkBehaviourId(Runner, nextServePlayer);
        _gameController.Rpc_SetServePlayer(playerRef);
    }

    IEnumerator DroppedBallDespawn()
    {
        // 1초 경과 후 Despawn 실행
        yield return new WaitForSeconds(1f);

        Debug.Log($"바닥에 떨어진 공 디스폰: {Object.Id}");
        Runner.Despawn(Object);
    }

}
