using System.Collections.Generic;
using UnityEngine;
using TMPro;
using Fusion;

namespace Asteroids.SharedSimple
{
    public class ReadyUIController : MonoBehaviour
    {
        public GameObject playerListPanel;
        public GameObject playerEntryPrefab;

        private Dictionary<PlayerRef, GameObject> playerEntries = new Dictionary<PlayerRef, GameObject>();

        public void AddPlayerOnEntryUI(PlayerRef player, PlayerDataNetworked playerDataNetworked)
        {
            // 이미 포함된 플레이어 정보 제외
            if (playerEntries.ContainsKey(player)) return;

            if (playerDataNetworked == null)
            {
                Debug.LogError("PlayerDataNetworked 인스턴스를 찾을 수 없습니다!");
                return;
            }

            // UI요소 공간 생성
            GameObject playerEntry = Instantiate(playerEntryPrefab, playerListPanel.transform);
            playerEntry.GetComponent<RectTransform>().anchoredPosition = new Vector2(0, -150 * playerEntries.Count); // 각 항목을 적절히 배치
            playerEntry.GetComponent<RectTransform>().sizeDelta = new Vector2(300, 50); // 크기 조정

            // UI에 플레이어 정보 표시
            TextMeshProUGUI playerNameText = playerEntry.transform.Find("PlayerNameText").GetComponent<TextMeshProUGUI>();
            if (playerNameText == null)
            {
                Debug.LogError("PlayerNameText를 찾을 수 없습니다!");
                return;
            }
            playerNameText.text = "Player" + player.PlayerId + " " + playerDataNetworked.NickName;

            // Player 정보를 playerEntry Dictionary에 추가
            playerEntries[player] = playerEntry;
            Debug.Log("대기열에 Player 추가됨: " + player.PlayerId);
        }
    }
}