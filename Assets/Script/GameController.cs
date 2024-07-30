using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Fusion;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class GameController : NetworkBehaviour, IPlayerJoined
{
    enum GamePhase
    {
        Ready,
        Starting,
        Running,
        Ending
    }

    // 게임 시작 버튼
    [SerializeField] private Button _startButton;


    [SerializeField] private float _startDelay = 4.0f;
    [SerializeField] private float _endDelay = 4.0f;

    private TickTimer _dontCheckforWinTimer;

    // 게임 상태 관리
    [Networked] private TickTimer Timer { get; set; }
    [Networked] private GamePhase Phase { get; set; }
    [Networked] private NetworkBehaviourId Winner { get; set; }
    // PingPong 순서 관리
    [Networked] public PlayerRef ServePlayerId { get; private set; }
    [Networked] public int ServingCount { get; private set; } = 0;
    [Networked] public int ServingPlayerIndex { get; set; } = 0;
    [Networked] public NetworkBool CanServiceBall { get; set; } = false;

    // 게임 시작 확인
    public bool GameIsRunning => Phase == GamePhase.Running;

    // 플레이어 생성 위치
    private SpawnPoint[] _spawnPoints;

    // 플레이어 데이터 관리, 변경 감지
    private List<NetworkBehaviourId> _playerDataNetworkedIds = new List<NetworkBehaviourId>();

    // 싱글톤 선언
    private static GameController _singleton;

    public static GameController Singleton
    {
        get => _singleton;
        private set
        {
            if (_singleton != null)
            {
                throw new InvalidOperationException();
            }
            _singleton = value;
        }
    }

    private void Awake()
    {
        // MasterClientObject 플래그 설정: 서버에서만 관리되며 클라이언트에는 동기화되지 않음
        // [Networked] 속성은 마스터 클라이언트에서만 조작 & non-Networked 속성은 로컬 클라이언트에서 조작 가능
        // 사용목적: 네트워크 동기화를 줄여 부하 감소
        GetComponent<NetworkObject>().Flags |= NetworkObjectFlags.MasterClientObject;
        Singleton = this;
    }

    private void OnDestroy()
    {
        if (Singleton == this)
        {
            _singleton = null;
        }
        else
        {
            throw new InvalidOperationException();
        }

    }

    public override void Spawned()
    {
        if (Object.HasStateAuthority)
        {
            // Initialize the game state on the master client
            Phase = GamePhase.Ready;
        }
    }

    public override void Render()
    {
        // Update the game display with the information relevant to the current game phase
        switch (Phase)
        {
            case GamePhase.Ready:
                UpdateReadyDisplay();
                break;
            case GamePhase.Starting:
                UpdateStartingDisplay();
                break;
            case GamePhase.Running:
                UpdateRunningDisplay();
                if (HasStateAuthority)
                {
                    CheckIfGameHasEnded();
                }
                break;
            case GamePhase.Ending:
                UpdateEndingDisplay();
                break;
            default:
                throw new ArgumentOutOfRangeException();
        }
    }

    private void UpdateReadyDisplay()
    {
        FindObjectOfType<PlayerSpawner>().StartPlayerSpawner();
        // 플레이어 생성 후 게임 Phase 변경
        Phase = GamePhase.Starting;
    }

    private void UpdateStartingDisplay()
    {
        if (_playerDataNetworkedIds.Count > 1)
        {
            _startButton.GetComponent<Button>().enabled = true;
        }
    }

    private void UpdateRunningDisplay()
    {

    }

    private void UpdateEndingDisplay()
    {

    }

    public void CheckIfGameHasEnded()
    {
        GameHasEnded();
    }

    private void GameHasEnded()
    {
        Phase = GamePhase.Ending;
    }

    public void PlayerJoined(PlayerRef player)
    {
        _dontCheckforWinTimer = TickTimer.CreateFromSeconds(Runner, 5);

        // 일정 시간 대기 후에 AddPlayer 호출
        StartCoroutine(DelayedAddPlayer(player));
    }

    private IEnumerator DelayedAddPlayer(PlayerRef player)
    {
        yield return new WaitForSeconds(1f); // 1초 대기

        var networkObject = Runner.GetPlayerObject(player);
        if (networkObject != null)
        {
            var playerDataNetworked = networkObject.GetComponent<PlayerDataNetworked>();
            if (playerDataNetworked != null)
            {
                Debug.Log($"PlayerDataNetworked 찾음: {playerDataNetworked.NickName}");

                // GameController의 PlayerDataList에 없다면 PlayerData 추가
                if (!_playerDataNetworkedIds.Contains(playerDataNetworked))
                {
                    _playerDataNetworkedIds.Add(playerDataNetworked);
                }

                // FindObjectOfType<ReadyUIController>().AddPlayerOnEntryUI(player, playerDataNetworked);
                Debug.Log("PlayerJoined");
            }
            else
            {
                Debug.LogError("PlayerDataNetworked 컴포넌트를 찾을 수 없습니다: " + player.PlayerId);
            }
        }
        else
        {
            Debug.LogError("NetworkObject를 찾을 수 없습니다: " + player.PlayerId);
        }

        PrintPlayerDataList();
    }


    // PlayerDataList에 저장된 목록 출력(디버깅용)
    private void PrintPlayerDataList()
    {
        Debug.Log($"리스트에 있는 플레이어 수: {_playerDataNetworkedIds.Count}");
        foreach (var playerData in _playerDataNetworkedIds)
        {
            if (Runner.TryFindBehaviour(playerData, out PlayerDataNetworked playerDataNetworkedComponent))
            {
                Debug.Log($"저장된 플레이어 정보: {playerDataNetworkedComponent.NickName} {playerDataNetworkedComponent.Wins} {playerDataNetworkedComponent.Score}");
            }
        }
    }

    internal List<NetworkBehaviourId> GetPlayerDataNetworkedIds()
    {
        return _playerDataNetworkedIds;
    }

    [Rpc(RpcSources.All, RpcTargets.All)]
    public void RPC_GameStartButtonOn()
    {
        CanServiceBall = true;
        // Photon Fusion2 에서 네트워크 오브젝트Id/BehaviourId에서 PlayerRef를 가져오기 위해 정의한 Static Class: NetworkUtilities
        ServePlayerId = NetworkUtilities.GetPlayerRefFromNetworkBehaviourId(Runner, _playerDataNetworkedIds[ServingPlayerIndex]);
        GameObject.Find("StartUI").SetActive(false);
    }


    // public void ChangeServePlayer()
    // {
    //     foreach (var localPlayerData in _playerDataNetworkedIds)
    //     {
    //         if (Runner.TryFindBehaviour(localPlayerData, out PlayerDataNetworked playerDataNetworkedComponent))
    //         {
    //             if(!ServePlayerName.Equals(playerDataNetworkedComponent.NickName))
    //             {
    //                 ServePlayerName = playerDataNetworkedComponent.NickName;
    //             }
    //         }
    //     }
    // }
}