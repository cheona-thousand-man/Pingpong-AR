using System;
using System.Collections.Generic;
using System.Linq;
using Fusion;
using TMPro;
using UnityEngine;

namespace Asteroids.SharedSimple
{
	public class GameController : NetworkBehaviour, IPlayerJoined
	{
		enum GamePhase
		{
			Ready,
			Starting,
			Running,
			Ending
		}

		[SerializeField] private float _startDelay = 4.0f;
		[SerializeField] private float _endDelay = 4.0f;

		private TickTimer _dontCheckforWinTimer;

		[Networked] private TickTimer Timer { get; set; }
		[Networked] private GamePhase Phase { get; set; }
		[Networked] private NetworkBehaviourId Winner { get; set; }

		public bool GameIsRunning => Phase == GamePhase.Running;

		private SpawnPoint[] _spawnPoints;

		private List<NetworkBehaviourId> _playerDataNetworkedIds = new List<NetworkBehaviourId>();

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

				// 준비 버튼 활성화
				Debug.Log("준비 버튼 활성화를 해보자!");
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
			FindObjectOfType<PlayerSpawner>().StartPlayerSpawner(this);
			Debug.Log("계속 생성되는 ");
			Phase = GamePhase.Starting;
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

		public void TrackNewPlayer(NetworkBehaviourId playerDataNetworkedId)
		{
			_playerDataNetworkedIds.Add(playerDataNetworkedId);
		}

		public void PlayerJoined(PlayerRef player)
		{
			_dontCheckforWinTimer = TickTimer.CreateFromSeconds(Runner, 5);
		}
	}
}