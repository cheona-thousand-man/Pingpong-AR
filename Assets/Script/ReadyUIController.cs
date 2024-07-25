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

        public void AddPlayer(PlayerRef player, PlayerDataNetworked playerDataNetworked)
        {
            Debug.Log("AddPlayer 호출됨: " + player.PlayerId);

            if (playerEntries.ContainsKey(player)) return;

            if (playerDataNetworked == null)
            {
                Debug.LogError("PlayerDataNetworked 인스턴스를 찾을 수 없습니다!");
                return;
            }

            GameObject playerEntry = Instantiate(playerEntryPrefab, playerListPanel.transform);
            playerEntry.GetComponent<RectTransform>().anchoredPosition = new Vector2(0, -50 * playerEntries.Count); // 각 항목을 적절히 배치
            playerEntry.GetComponent<RectTransform>().sizeDelta = new Vector2(300, 50); // 크기 조정

            Debug.Log("playerEntry 인스턴스화됨: " + playerEntry.name);

            TextMeshProUGUI playerNameText = playerEntry.transform.Find("PlayerNameText").GetComponent<TextMeshProUGUI>();
            if (playerNameText == null)
            {
                Debug.LogError("PlayerNameText를 찾을 수 없습니다!");
                return;
            }

            playerNameText.text = "Player" + player.PlayerId + " " + playerDataNetworked.NickName;
            Debug.Log("PlayerNameText 설정됨: " + playerNameText.text);

            playerEntries[player] = playerEntry;
            Debug.Log("Player 추가됨: " + player.PlayerId);
        }
    }
}