using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Fusion;
using TMPro;
using UnityEngine.SceneManagement;

public class StartMenu : MonoBehaviour
{
    [SerializeField] private NetworkRunner _networkRunnerPrefab = null;
    [SerializeField] private TMP_InputField _playerName = null;
    [SerializeField] private TextMeshProUGUI _playerNamePlaceholder = null;
    [SerializeField] private TMP_InputField _roomName = null;
    [SerializeField] private string _gameScenePath = null;

    private NetworkRunner _runnerInstance = null;

    public void StartShared()
    {
        SetPlayerData();
        if (_roomName == null)
        {
            StartGame(GameMode.Shared, "default", _gameScenePath);
        }
        else
        {
            StartGame(GameMode.Shared, _roomName.text, _gameScenePath);
        }
    }

    private void SetPlayerData()
    {
        if (string.IsNullOrWhiteSpace(_playerName.text))
        {
            PlayerData.PlayerName = _playerNamePlaceholder.text;
        }
        else
        {
            PlayerData.PlayerName = _playerName.text;
        }
    }

    private void StartGame(GameMode mode, string roomName, string sceneName)
    {
        _runnerInstance = FindObjectOfType<NetworkRunner>();
        if (_runnerInstance == null)
        {
            _runnerInstance = Instantiate(_networkRunnerPrefab);
        }

        _runnerInstance.ProvideInput = true;

        var startGameArgs = new StartGameArgs()
        {
            GameMode = mode,
            SessionName = roomName,
            Scene = SceneRef.FromIndex(SceneUtility.GetBuildIndexByScenePath(_gameScenePath)),
            ObjectProvider = default
        };
        Debug.Log($"Joined Player Name: {PlayerData.PlayerName}");
    }
}
