using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class PhotonRPC : MonoBehaviourPun
{
    public PhotonView ThisPV;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        ThisPV.RPC("distanceValue", RpcTarget.Others, GameManager.Instance.distanceText.text);
    }

    [PunRPC]
    public void distanceValue(string Testing)
    {
        GameManager.Instance.distanceTextEnemy.text = Testing;
    }
}
