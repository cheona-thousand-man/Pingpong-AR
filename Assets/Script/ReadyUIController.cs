using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Fusion;

public class ReadyUIController : MonoBehaviour
{
    public GameObject playerListPanel;
    public GameObject playerEntryPrefab;
    [Networked] private int playerCount { get; set; } = 0;

    [SerializeField] private Button myButton;

    private Dictionary<PlayerRef, GameObject> playerEntries = new Dictionary<PlayerRef, GameObject>();

    public void Start()
    {
        myButton.interactable = false;
        myButton.onClick.AddListener(OnMyButtonClick);
    }

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

        playerCount++;

        Debug.Log("Player 숫자 " + playerCount);

        if (playerCount == 2)
        {
            myButton.interactable = true;
        }
    }

    private void OnMyButtonClick()
    {
        // 버튼 클릭 시 실행할 코드
        playerListPanel.SetActive(false);
        playerEntryPrefab.SetActive(false);
        myButton.gameObject.SetActive(false);
        Debug.Log("playerListPanel과 playerEntryPrefab이 비활성화되었습니다.");
    }
}
