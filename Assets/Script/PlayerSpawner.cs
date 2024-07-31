using Fusion;
using UnityEngine;

// Therefore none of its parameters need to be [Networked].
public class PlayerSpawner : NetworkBehaviour
{
    // References to the NetworkObject prefab to be used for the players' object.
    [SerializeField] private NetworkPrefabRef _playerPrefab = NetworkPrefabRef.Empty;
    public GameObject player1Camera;
    public GameObject player2Camera;

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

        Debug.Log("Player id" + player.PlayerId);

        if (player.PlayerId == 1) {
            Debug.Log("1번 호출");

            player1Camera.SetActive(true);
            player2Camera.SetActive(false);
            Quaternion rotation = Quaternion.Euler(0, 0, 0);
            var playerObject = Runner.Spawn(_playerPrefab, spawnPosition, rotation, player);
            // Set Player Object to facilitate access across systems.
            Runner.SetPlayerObject(player, playerObject);
        }

        if (player.PlayerId == 2)
        {
            Debug.Log("2번 호출됨");

            player1Camera.SetActive(false);
            player2Camera.SetActive(true);
            var playerObject = Runner.Spawn(_playerPrefab, spawnPosition, Quaternion.identity, player);
            // Set Player Object to facilitate access across systems.
            Runner.SetPlayerObject(player, playerObject);

            playerObject.transform.rotation = Quaternion.Euler(0, 0, 0);
        }
    }
}