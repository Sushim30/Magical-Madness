using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.InputSystem;
using Photon.Pun;

public class Timer : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI timerText;
    [SerializeField] float remainingTime;
    public GameObject matchOverObject;
    public PlayerInput playerinput;
    PlayerManager playermanager;
   
    void Update()
    {
        if(remainingTime > 0)
        {
            remainingTime -= Time.deltaTime;
        }else if(remainingTime < 0)
        {
            remainingTime = 0;
            matchOverObject.SetActive(true);
            playerinput.enabled = false;
            PhotonNetwork.LoadLevel(2);
            PhotonNetwork.Destroy(playermanager.controller);
        }
        int minutes = Mathf.FloorToInt(remainingTime / 60);
        int seconds = Mathf.FloorToInt(remainingTime % 60);
        timerText.text = string.Format("{0:00}:{1:00}",minutes,seconds);
    }
}
