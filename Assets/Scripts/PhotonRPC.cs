using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Newtonsoft.Json;
using TMPro;
using Photon.Realtime;

public class PhotonRPC : MonoBehaviourPunCallbacks
{
    public PhotonView ThisPV;

    public GameObject textPrefab;  // Assign a TextMeshProUGUI prefab in the Inspector
    public Transform parentPanel; // Assign the UI panel where text fields will be added
    public GameObject newText;

    Dictionary<string, string> playerDistances = new Dictionary<string, string>();
    Dictionary<string, TextMeshProUGUI> playerTextObjects = new Dictionary<string, TextMeshProUGUI>();

    void Update()
    {
        //Debug.Log("ROom" + PhotonNetwork.CurrentRoom.PlayerCount);
        string playerName = OnlyData.Data.Playername;
        string playerDistance = GameManager.Instance.MyDisdanceValue;

        // Update player's own distance
        playerDistances[playerName] = playerDistance;

        // Convert Dictionary to JSON and send to other players
        string jsonData = JsonConvert.SerializeObject(playerDistances);
        ThisPV.RPC("ReceivePlayerDistances", RpcTarget.Others, jsonData);
    }

    [PunRPC]
    public void ReceivePlayerDistances(string jsonData)
    {
        Dictionary<string, string> receivedDict = JsonConvert.DeserializeObject<Dictionary<string, string>>(jsonData);

        foreach (var entry in receivedDict)
        {
            string playerName = entry.Key;
            string distance = entry.Value;

            // Check if a UI Text object already exists for this player
            if (!playerTextObjects.ContainsKey(playerName))
            {
                Debug.Log("CHECK CLONE");
                // Create a new Text object for this player
                 newText = Instantiate(textPrefab, parentPanel);
                TextMeshProUGUI textComponent = newText.GetComponent<TextMeshProUGUI>();

                // Store it in the dictionary for future updates
                playerTextObjects[playerName] = textComponent;
            }

            // Update the player's distance UI
            playerTextObjects[playerName].text = $"{playerName}: {distance} meters";
        }
    }

    public override void OnPlayerLeftRoom(Player otherPlayer)
    {
        Debug.Log("Remaining Players: " + PhotonNetwork.CurrentRoom.PlayerCount);
        string playerName = otherPlayer.NickName;

        // Check if the player UI object exists
        if (playerTextObjects.ContainsKey(playerName))
        {
            // Destroy the UI element
            Destroy(playerTextObjects[playerName].gameObject);

            // Remove from dictionary
            playerTextObjects.Remove(playerName);
        }

        Debug.Log(playerName + " left the room. UI updated.");


        if (PhotonNetwork.CurrentRoom.PlayerCount == 1)
        {
            Debug.Log("m / <color=yellow> ++++ YOU WIN ++++ </color>");
            PhotonNetwork.LoadLevel(0);
        }
    }

}

