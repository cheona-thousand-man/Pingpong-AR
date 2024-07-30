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
    [SerializeField] private GameObject _startUI;
    [SerializeField] private Button _startButton;

    // 중복 실행되지 않도록 하는 TickTimer
    private TickTimer _dontCheckforWinTimer;

    // 게임 상태 관리
    [Networked] private TickTimer Timer { get; set; }
    [Networked] private GamePhase Phase { get; set; }
    [Networked] private NetworkBehaviourId Winner { get; set; }
    // 플레이어 데이터 관리
    [Networked] public NetworkBehaviourId Player1 { get; private set; }
    [Networked] public NetworkBehaviourId Player2 { get; private set; }
    // PingPong 순서 관리
    [Networked] public PlayerRef ServePlayerId { get; private set; }
    [Networked] public int ServingCount { get; set; } = 0;
    [Networked] public NetworkBool CanServiceBall { get; set; } = false;

    // 게임 시작 확인
    public bool GameIsRunning => Phase == GamePhase.Running;

    // 플레이어 생성 위치
    private SpawnPoint[] _spawnPoints;
    // 플레이어 생성 확인
    private bool isPlayerSpawned = false;

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
        Debug.Log($"Current Phase: {Phase}");
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
        // 본인의 플레이어 오브젝트가 없는 경우에만 Player spawn
        if (!isPlayerSpawned)
        {
            Debug.Log($"플레이어 오브젝트 스폰: {Runner.LocalPlayer}");
            FindObjectOfType<PlayerSpawner>().SpawnPlayer(Runner.LocalPlayer);
            isPlayerSpawned = true;
        }

        // 모두 입장할 경우 Phase 변경
        if (Player1.IsValid && Player2.IsValid)
        {
            Phase = GamePhase.Starting;

            // 마스터 클라이언트 게임 시작 UI 보이기 및 버튼 활성화
            if (Runner.IsSharedModeMasterClient)
                {
                    _startUI.SetActive(true);
                    _startButton.GetComponent<Button>().enabled = true;
                    _startButton.GetComponentInChildren<TextMeshProUGUI>().text = "StartGame";
                }
            else
                {
                    PrintPlayerData();
                }
        }
    }

    private void UpdateStartingDisplay()
    {
        
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
        Debug.Log($"PlayerJoined 호출");
        _dontCheckforWinTimer = TickTimer.CreateFromSeconds(Runner, 5);

        // 캐릭터 생성될 때까지 대기 후에 AddPlayer 호출
        StartCoroutine(DelayedAddPlayer(player));
    }

    private IEnumerator DelayedAddPlayer(PlayerRef player)
    {
        NetworkObject networkObject = null;

        yield return new WaitUntil(() => Runner.TryGetPlayerObject(player, out networkObject)); // 플레이어 오브젝트 생성까지 대기

        var playerDataNetworked = networkObject.GetComponent<PlayerDataNetworked>();
        if (playerDataNetworked != null)
        {
            Debug.Log($"PlayerDataNetworked 찾음: {playerDataNetworked.NickName}");

            // GameController의 Player 목록에 추가
            // 마스터 클라이언트이고, 지금 Player가 플레이어 오브젝트 소유자일 때
            if (Runner.IsSharedModeMasterClient && Runner.LocalPlayer == NetworkUtilities.GetPlayerRefFromNetworkBehaviourId(Runner, playerDataNetworked.Id))
            {
                Player1 = playerDataNetworked.Id;
            }
            else // 로컬 클라이언트일 때
            {
                Player2 = playerDataNetworked.Id;
            }

            // FindObjectOfType<ReadyUIController>().AddPlayerOnEntryUI(player, playerDataNetworked);
            Debug.Log("PlayerJoined");
        }
        else
        {
            Debug.LogError("PlayerDataNetworked 컴포넌트를 찾을 수 없습니다: " + player.PlayerId);
        }

        PrintPlayerData();
    }


    // PlayerDataList에 저장된 목록 출력(디버깅용)
    private void PrintPlayerData()
    {
        if (Player1.IsValid && Runner.TryFindBehaviour(Player1, out PlayerDataNetworked playerDataNetworkedComponent))
            {
                Debug.Log($"저장된 플레이어1 정보: {playerDataNetworkedComponent.NickName} {playerDataNetworkedComponent.Wins} {playerDataNetworkedComponent.Score}");
            }

        if (Player2.IsValid && Runner.TryFindBehaviour(Player2, out PlayerDataNetworked playerDataNetworkedComponent2))
            {
                Debug.Log($"저장된 플레이어2 정보: {playerDataNetworkedComponent2.NickName} {playerDataNetworkedComponent2.Wins} {playerDataNetworkedComponent2.Score}");
            }
    }

    // 첫 순서가 마스터 클라이언트(플레이어1)이므로 RPC 미사용
    public void GameStartButtonOn()
    {
        _startUI.SetActive(false); // 비활성화
        CanServiceBall = true;
        ServePlayerId = NetworkUtilities.GetPlayerRefFromNetworkBehaviourId(Runner, Player1);
    }

}