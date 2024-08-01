using Fusion;
using UnityEngine;

public class PlayerSpawner : NetworkBehaviour
{
    [SerializeField] private NetworkPrefabRef _playerPrefab = NetworkPrefabRef.Empty;
    [SerializeField] private NetworkPrefabRef _playerModelPrefab = NetworkPrefabRef.Empty;
    public GameObject player1Camera;
    public GameObject player2Camera;

    private SpawnPoint[] _spawnPoints = null;

    public override void Spawned()
    {
        // Collect all spawn points in the scene.
        _spawnPoints = FindObjectsOfType<SpawnPoint>();
    }

    public void SpawnPlayer(PlayerRef player)
    {
        int index = player.PlayerId % _spawnPoints.Length;
        var spawnPosition = _spawnPoints[index].transform.position;

        // 이상한 다른 프리팹을 가져오는 오류 방지
        if (_playerPrefab == NetworkPrefabRef.Empty || !_playerPrefab.Equals(_playerModelPrefab))
        {  
            _playerPrefab = _playerModelPrefab;
        }

        var playerObject = Runner.Spawn(_playerPrefab, spawnPosition, Quaternion.identity, player);

        if (player.PlayerId == 1)
        {
            player1Camera.SetActive(true);
            player2Camera.SetActive(false);
        }
        else if (player.PlayerId == 2)
        {
            player1Camera.SetActive(false);
            player2Camera.SetActive(true);
            // Ensure the rotation is applied instantly
            playerObject.transform.rotation = Quaternion.Euler(0, 180, 0);
        }

        // Set Player Object to facilitate access across systems.
        Runner.SetPlayerObject(player, playerObject);
    }
}