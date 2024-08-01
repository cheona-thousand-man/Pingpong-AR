using UnityEngine;
using Fusion;

// Holds the player's information and ensures it is replicated to all clients.
public class PlayerDataNetworked : NetworkBehaviour
{
    // Global static setting
    private const int STARTING_WINS = 0;
    private const int ENDING_WINS = 3;

    private ChangeDetector _changeDetector;

    // Game Session SPECIFIC Settings are used in the UI.
    // The method passed to the OnChanged attribute is called everytime the [Networked] parameter is changed.
    [HideInInspector]
    [Networked]
    public NetworkString<_16> NickName { get; private set; }

    [HideInInspector]
    [Networked]
    public int Wins { get; private set; }

    [HideInInspector]
    [Networked]
    public int Score { get; private set; }

    public override void Spawned()
    {

        // --- StateAuthority
        // Initialized game specific settings
        if (Object.HasStateAuthority)
        {
            Wins = STARTING_WINS;
            Score = 0;
            NickName = LocalPlayerData.NickName;
        }

        // 향후 UI 업데이트 작업을 위해서 ChangeDetector 가져오기
        _changeDetector = GetChangeDetector(ChangeDetector.Source.SimulationState);
    }

    // Increase the score by 1
    public void AddToScore()
    {
        Score++;
        Rpc_PlayerScoreUpdate();
    }

    // Increase the current Wins by 1
    public void AddToWin()
    {
        Wins++;
        Rpc_PlayerScoreUpdate();
    }

    [Rpc(RpcSources.All, RpcTargets.All)]
    private void Rpc_PlayerScoreUpdate()
    {
        Debug.Log($"플레이어 {NickName} 점수 획득: {Score} {Wins}");
    }
}