using Fusion;
using UnityEngine;

// Therefore none of its parameters need to be [Networked].
public class PlayerSpawner : NetworkBehaviour
{
    // References to the NetworkObject prefab to be used for the players' object.
    [SerializeField] private NetworkPrefabRef _playerPrefab = NetworkPrefabRef.Empty;

    private SpawnPoint[] _spawnPoints = null;

    public override void Spawned()
    {
        // Collect all spawn points in the scene.
        _spawnPoints = FindObjectsOfType<SpawnPoint>();
    }

    // Spawn a player.
    // The spawn point is chosen in the _spawnPoints array using the implicit playerRef to int conversion 
    public void SpawnPlayer(PlayerRef player)
    {
        // Modulo is used in case there are more players than spawn points.
        int index = player.PlayerId % _spawnPoints.Length;
        var spawnPosition = _spawnPoints[index].transform.position;

        var playerObject = Runner.Spawn(_playerPrefab, spawnPosition, Quaternion.identity, player);
        // Set Player Object to facilitate access across systems.
        Runner.SetPlayerObject(player, playerObject);
    }
}