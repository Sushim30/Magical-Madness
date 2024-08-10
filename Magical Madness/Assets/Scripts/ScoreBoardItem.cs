using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using Photon.Realtime;
using Photon.Pun;
using Hashtable = ExitGames.Client.Photon.Hashtable;

public class ScoreBoardItem : MonoBehaviourPunCallbacks
{
    public TMP_Text usernameText;
    public TMP_Text KillsText;
    public TMP_Text DeathText;

    Player player;

    public void Initialize(Player player)
    {
        this.player = player;
        usernameText.text = player.NickName;
        UpdateStats();
    }

    void UpdateStats()
    {
        if (player.CustomProperties.TryGetValue("kills", out object kills))
        {
            KillsText.text = kills.ToString();
        }
        else
        {
            KillsText.text = "0";
        }

        if (player.CustomProperties.TryGetValue("deaths", out object deaths))
        {
            DeathText.text = deaths.ToString();
        }
        else
        {
            DeathText.text = "0";
        }
    }

    public override void OnPlayerPropertiesUpdate(Player targetPlayer, Hashtable changedProps)
    {
        if (targetPlayer == player && (changedProps.ContainsKey("kills") || changedProps.ContainsKey("deaths")))
        {
            UpdateStats();
        }
    }
}
