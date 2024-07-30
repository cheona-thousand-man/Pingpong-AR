using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Fusion;
using TMPro;
using UnityEngine.SceneManagement;
using System;

public class StartMenu : MonoBehaviour
{
    [SerializeField] private NetworkRunner _networkRunnerPrefab = null;
    [SerializeField] private TMP_InputField _nickName = null;
    [SerializeField] private TextMeshProUGUI _nickNamePlaceholder = null;
    [SerializeField] private TMP_InputField _roomName = null;
    [SerializeField] private string _gameScenePath = null;

    // 단말기 별 네트워크 관리자
    private NetworkRunner _runnerInstance = null;
    // NickName 최대 길이: 16자
    private const int MAX_NAME_LENGTH = 16;

    public void StartShared()
    {
        SetPlayerData();
        if (_roomName == null)
        {
            JoinGame(GameMode.Shared, "default", _gameScenePath);
        }
        else
        {
            JoinGame(GameMode.Shared, _roomName.text, _gameScenePath);
        }
    }

    private void SetPlayerData()
    {
        if (string.IsNullOrWhiteSpace(_nickName.text))
        {
            // LocalPlayerData에서 입력이 없는 경우 플레이어 Name 자동으로 생성 구현
            // LocalPlayerData.NickName = _nickNamePlaceholder.text;
        }
        else
        {
            LocalPlayerData.NickName = _nickName.text.Substring(0, Mathf.Min(_nickName.text.Length, MAX_NAME_LENGTH));
        }
    }

    private void JoinGame(GameMode mode, string roomName, string sceneName)
    {
        _runnerInstance = FindObjectOfType<NetworkRunner>();
        if (_runnerInstance == null)
        {
            _runnerInstance = Instantiate(_networkRunnerPrefab);
        }

        _runnerInstance.ProvideInput = true;

        var JoinGameArgs = new StartGameArgs()
        {
            GameMode = mode,
            SessionName = roomName,
            Scene = SceneRef.FromIndex(SceneUtility.GetBuildIndexByScenePath(_gameScenePath)),
            ObjectProvider = default
        };
        Debug.Log($"Joined Player Name: {LocalPlayerData.NickName}");

        _runnerInstance.StartGame(JoinGameArgs);
    }
}
