﻿using UnityEngine;
using Photon.Pun;

public class CollidingObject : MonoBehaviourPun {

    [SerializeField]
    private int price;
    private void OnTriggerEnter2D(Collider2D collision) {
        if(collision.gameObject.CompareTag("Vehicle")) {
            //연료 획득 시
            if(gameObject.name.Contains("Fuel")) {  
                GameManager.Instance.FuelCharge();
                gameObject.SetActive(false);
            }
            
            //목표 도착지에 도달하여 게임 성공
            else if(gameObject.name.Contains("Goal")) {  
                GameManager.Instance.ReachGoal = true;
                PhotonRPC.Data.MasterSendMessage();
                GameManager.Instance.gameStateText.text = "<color=#FFFF4C>YOU WIN!!!</color>";
                GameManager.Instance.StartGameOver();
            }

            //코인 획득
            else if(gameObject.name.Contains("Coin")) {  
                GameManager.Instance.GetCoin(price);
                gameObject.SetActive(false);
            }
        }
    }

    [PunRPC]
    public void OtherGameOver()
    {
        Debug.Log("CheckClient -CALLING");
        GameManager.Instance.StartGameOver();
    }
}