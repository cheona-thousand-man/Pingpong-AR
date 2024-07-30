using UnityEngine;
using Fusion;


// Holds the player's information and ensures it is replicated to all clients.
public class PlayerDataNetworked : NetworkBehaviour
{
    // Global static setting
    private const int STARTING_WINS = 0;
    private const int ENDING_WINS = 3;

    private ChangeDetector _changeDetector;
    private PlayerScorePanel _overviewPanel = null;

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

        FindObjectOfType<GameController>().TrackNewPlayer(this);
        _overviewPanel = FindObjectOfType<PlayerScorePanel>();
        // Add an entry to the local Overview panel with the information of this spaceship
        _overviewPanel.AddEntry(Object.InputAuthority, this);

        // Refresh panel visuals in Spawned to set to initial values.
        _overviewPanel.UpdateEntry(this);

        // 향후 UI 업데이트 작업을 위해서 ChangeDetector 가져오기
        _changeDetector = GetChangeDetector(ChangeDetector.Source.SimulationState);
    }


    // Remove the entry in the local Overview panel for this spaceship
    public override void Despawned(NetworkRunner runner, bool hasState)
    {
        _overviewPanel.RemoveEntry(this);
    }

    public override void Render()
    {
        foreach (var change in _changeDetector.DetectChanges(this, out var previousBuffer, out var currentBuffer))
        {
            _overviewPanel.UpdateEntry(this);
            break;
        }
    }

    // Increase the score by 1
    public void AddToScore(int points)
    {
        Score += points;
    }

    // Increase the current Wins by 1
    public void AddToWin()
    {
        Wins++;
    }

    // RPC used to send player information to the Host
    [Rpc(sources: RpcSources.InputAuthority, targets: RpcTargets.StateAuthority)]
    private void RpcSetNickName(string nickName)
    {
        if (string.IsNullOrEmpty(nickName)) return;
        NickName = nickName;
    }
}
