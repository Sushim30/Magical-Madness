using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using Photon.Pun;
using UnityEngine.InputSystem;

public class Timer : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI timerText;
    [SerializeField] float remainingTime;
    public GameObject matchOverObject;
    public PlayerInput playerinput;
    public PlayerManager playermanager;

    private bool isTimerActive = true;

    void Update()
    {
        if (isTimerActive && remainingTime > 0)
        {
            remainingTime -= Time.deltaTime;
        }
        else if (isTimerActive && remainingTime <= 0)
        {
            remainingTime = 0;
            EndMatch();
        }

        int minutes = Mathf.FloorToInt(remainingTime / 60);
        int seconds = Mathf.FloorToInt(remainingTime % 60);
        timerText.text = string.Format("{0:00}:{1:00}", minutes, seconds);
    }

    void EndMatch()
    {
        isTimerActive = false;
        matchOverObject.SetActive(true);
        playerinput.enabled = false;

        // Ensure proper cleanup
        if (playermanager != null && playermanager.controller != null)
        {
            PhotonNetwork.Destroy(playermanager.controller);
        }

        PhotonNetwork.LoadLevel(2);
    }
}
