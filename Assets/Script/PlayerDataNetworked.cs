using UnityEngine;
using Fusion;

namespace Asteroids.SharedSimple
{
    // Holds the player's information and ensures it is replicated to all clients.
    public class PlayerDataNetworked : NetworkBehaviour
    {
        // Global static setting
        private const int WINS = 3;

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
                Wins = REMAIN_WINS;
                Score = 0;
                NickName = LocalPlayerData.NickName;
            }


            _changeDetector = GetChangeDetector(ChangeDetector.Source.SimulationState);
        }

        // Increase the score by X amount of points
        public void AddToScore(int points)
        {
            Score += points;
        }

        // Decrease the current Wins by 1
        public void SubtractLife()
        {
            Wins--;
        }

        // RPC used to send player information to the Host
        [Rpc(sources: RpcSources.InputAuthority, targets: RpcTargets.StateAuthority)]
        private void RpcSetNickName(string nickName)
        {
            if (string.IsNullOrEmpty(nickName)) return;
            NickName = nickName;
        }
    }
}