using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Realtime;
using Photon.Pun;
using TMPro;
using System.Linq;

public class ScoreBoard : MonoBehaviourPunCallbacks
{
    [SerializeField] Transform container;
    [SerializeField] GameObject scoreboardItemPrefab;
    [SerializeField] CanvasGroup CanvasGroup;
    [SerializeField] GameObject textHolder;

    Dictionary<Player, ScoreBoardItem> scoreboardItems = new Dictionary<Player, ScoreBoardItem>();

    private void Start()
    {
        // Shuffle the players list for random initialization
        List<Player> playerList = PhotonNetwork.PlayerList.OrderBy(p => Random.value).ToList();
        foreach (Player player in playerList)
        {
            AddScoreboardItems(player);
            textHolder.SetActive(false);
        }
    }

    void AddScoreboardItems(Player player)
    {
        ScoreBoardItem item = Instantiate(scoreboardItemPrefab, container).GetComponent<ScoreBoardItem>();
        item.Initialize(player);
        scoreboardItems[player] = item;
    }

    void RemoveScoreboardItem(Player player)
    {
        Destroy(scoreboardItems[player].gameObject);
        scoreboardItems.Remove(player);
    }

    public override void OnPlayerEnteredRoom(Player newPlayer)
    {
        AddScoreboardItems(newPlayer);
    }

    public override void OnPlayerLeftRoom(Player otherPlayer)
    {
        RemoveScoreboardItem(otherPlayer);
    }

    private void Update()
    {
        if (Input.GetKey(KeyCode.Tab))
        {
            CanvasGroup.alpha = 1;
            textHolder.SetActive(true);
        }
        else
        {
            CanvasGroup.alpha = 0;
            textHolder.SetActive(false);
        }
    }

    public void UpdateScoreboard()
    {
        // Sort the players based on kills and deaths
        List<Player> sortedPlayers = PhotonNetwork.PlayerList
            .OrderByDescending(p => p.CustomProperties.ContainsKey("kills") ? (int)p.CustomProperties["kills"] : 0)
            .ThenBy(p => p.CustomProperties.ContainsKey("deaths") ? (int)p.CustomProperties["deaths"] : 0)
            .ToList();

        // Remove all current items
        foreach (Transform child in container)
        {
            Destroy(child.gameObject);
        }

        scoreboardItems.Clear();

        // Add sorted items
        foreach (Player player in sortedPlayers)
        {
            AddScoreboardItems(player);
        }
    }
}
