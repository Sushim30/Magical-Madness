using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using Photon.Pun;
using Photon.Realtime;
using System.Linq;
using Hashtable = ExitGames.Client.Photon.Hashtable;

public class PlayerManager : MonoBehaviour
{
    PhotonView PV;
    public GameObject controller;
    Animator animator;
    int kills;
    int deaths;

    private void Awake()
    {
        // Initialize PhotonView and Animator components
        PV = GetComponent<PhotonView>();
        animator = GetComponent<Animator>();
    }

    private void Start()
    {
        // Check if the PhotonView belongs to this player
        if (PV.IsMine)
        {
            CreateController();
        }
    }

    // Create the player controller at a spawn point
    void CreateController()
    {
        Transform spawnpoint = SpawnManager.instance?.GetSpawnPoint();
        if (spawnpoint == null)
        {
            Debug.LogError("Spawn point not found!");
            return;
        }

        // Instantiate the player controller using PhotonNetwork
        controller = PhotonNetwork.Instantiate(Path.Combine("PhotonPrefabs", "PlayerController"), spawnpoint.position, spawnpoint.rotation, 0, new object[] { PV.ViewID });
    }

    // Handle player death
    public void Die()
    {
        if (controller != null)
        {
            PhotonNetwork.Destroy(controller);
        }

        deaths++;

        // Update player properties to include the new death count
        Hashtable hash = new Hashtable();
        hash.Add("deaths", deaths);
        PhotonNetwork.LocalPlayer.SetCustomProperties(hash);

        // Start the respawn coroutine with a delay
        StartCoroutine(Respawner(3f));
    }

    // Coroutine to respawn the player after a delay
    private IEnumerator Respawner(float delay)
    {
        yield return new WaitForSeconds(delay);

        Transform spawnpoint = SpawnManager.instance?.GetSpawnPoint();
        if (spawnpoint == null)
        {
            Debug.LogError("Spawn point not found!");
            yield break;
        }

        // Instantiate the player controller at the new spawn point
        controller = PhotonNetwork.Instantiate(Path.Combine("PhotonPrefabs", "PlayerController"), spawnpoint.position, spawnpoint.rotation, 0, new object[] { PV.ViewID });
    }

    // Handle player kills
    public void GetKill()
    {
        PV.RPC(nameof(RPC_GetKill), PV.Owner);
    }

    // Remote Procedure Call to update the kill count
    [PunRPC]
    public void RPC_GetKill()
    {
        kills++;

        // Update player properties to include the new kill count
        Hashtable hash = new Hashtable();
        hash.Add("kills", kills);
        PhotonNetwork.LocalPlayer?.SetCustomProperties(hash);
    }

    // Find a PlayerManager instance for a given player
    public static PlayerManager Find(Player player)
    {
        return FindObjectsOfType<PlayerManager>().SingleOrDefault(x => x.PV.Owner == player);
    }
}
