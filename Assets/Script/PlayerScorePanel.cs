using System;
using System.Collections.Generic;
using Fusion;
using TMPro;
using UnityEngine;

public class PlayerScorePanel : MonoBehaviour
{

    [SerializeField] private TextMeshProUGUI _playerOverviewEntryPrefab = null;

    private Dictionary<PlayerDataNetworked, TextMeshProUGUI> _playerListEntries =
        new Dictionary<PlayerDataNetworked, TextMeshProUGUI>();

    public void Clear()
    {
        foreach (var tmp in _playerListEntries.Values)
        {
            Destroy(tmp);
        }

        _playerListEntries.Clear();
    }

    public void AddEntry(PlayerRef playerRef, PlayerDataNetworked playerDataNetworked)
    {
        Debug.Log("Add Entry");
        if (_playerListEntries.ContainsKey(playerDataNetworked)) return;
        if (playerDataNetworked == null) return;

        var entry = Instantiate(_playerOverviewEntryPrefab, this.transform);
        entry.transform.localScale = Vector3.one;

        _playerListEntries.Add(playerDataNetworked, entry);

        UpdateEntry(playerDataNetworked);
    }

    public void UpdateEntry(PlayerDataNetworked playerData)
    {
        Debug.Log("UpdateEntry");
        if (_playerListEntries.TryGetValue(playerData, out TextMeshProUGUI entry))
        {
            entry.text = $"Player1: {playerData.Score}";
        }
    }


    public void RemoveEntry(PlayerDataNetworked playerData)
    {
        Debug.Log("RemoveEntry");
        if (_playerListEntries.TryGetValue(playerData, out var entry) == false) return;

        if (entry != null)
        {
            Destroy(entry.gameObject);
        }

        _playerListEntries.Remove(playerData);
    }

}
