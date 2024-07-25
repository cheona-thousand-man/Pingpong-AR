using Fusion;
using UnityEngine;

// Therefore none of its parameters need to be [Networked].
public class PlayerSpawner : NetworkBehaviour
{
    [Networked] private bool _gameIsReady { get; set; } = false;

    // References to the NetworkObject prefab to be used for the players' spaceships.
    [SerializeField] private NetworkPrefabRef _playerPrefab = NetworkPrefabRef.Empty;

    private SpawnPoint[] _spawnPoints = null;

    public override void Spawned()
    {
        // Collect all spawn points in the scene.
        _spawnPoints = FindObjectsOfType<SpawnPoint>();

        // When the SpaceshipSpawner gets spawned on a late joiner, spawn a spaceship for them
        if (_gameIsReady)
        {
            SpawnPlayer(Runner.LocalPlayer);
        }
    }

    // The spawner is started when the GameController switches to GameState.Running.
    public void StartPlayerSpawner()
    {
        _gameIsReady = true;
        RpcInitialSpaceshipSpawn();
        Debug.Log("StartPlayerSpawner 호출");
    }

    [Rpc(RpcSources.StateAuthority, RpcTargets.All)]
    private void RpcInitialSpaceshipSpawn()
    {
        SpawnPlayer(Runner.LocalPlayer);
    }

    // Spawn a player.
    // The spawn point is chosen in the _spawnPoints array using the implicit playerRef to int conversion 
    private void SpawnPlayer(PlayerRef player)
    {
        // Modulo is used in case there are more players than spawn points.
        int index = player.PlayerId % _spawnPoints.Length;
        var spawnPosition = _spawnPoints[index].transform.position;

        var playerObject = Runner.Spawn(_playerPrefab, spawnPosition, Quaternion.identity, player);
        // Set Player Object to facilitate access across systems.
        Runner.SetPlayerObject(player, playerObject);
    }
}