using UnityEngine;
using PlayFab;
using PlayFab.ClientModels;
using System.Collections.Generic;
using UnityEngine.UI;
using TMPro;

public class PlayFabManager : MonoBehaviour
{
    public GameObject rowPrefab; // Prefab for leaderboard row
    public Transform rowsParent; // Parent container for leaderboard rows

    private string playFabId; // Store PlayFab ID after login
    public static PlayFabManager instance;

    private int earnedvalue;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else if (instance != this)
        {
            Destroy(gameObject); // Destroy duplicate instance
        }
    }

    void Start()
    {
        Login();
    }

    // 🔹 Automatically log in the player (Guest Login)
    public void Login()
    {
        var request = new LoginWithCustomIDRequest
        {
            CustomId = SystemInfo.deviceUniqueIdentifier,
            CreateAccount = true // Creates a new account if one doesn't exist
        };

        PlayFabClientAPI.LoginWithCustomID(request, OnLoginSuccess, OnError);
    }

    void OnLoginSuccess(LoginResult result)
    {
        playFabId = result.PlayFabId;
        Debug.Log("Login successful! PlayFab ID: " + playFabId);
    }
    public void SetDisplayName()
    {
        var request = new UpdateUserTitleDisplayNameRequest
        {
            DisplayName = GuestLogin.instance.statusText.text
        };

        PlayFabClientAPI.UpdateUserTitleDisplayName(request, OnDisplayNameUpdated, NameOnError);
    }

    private void OnDisplayNameUpdated(UpdateUserTitleDisplayNameResult result)
    {
        Debug.Log("Display Name Updated Successfully: " + result.DisplayName);
    }

    private void NameOnError(PlayFabError error)
    {
        Debug.LogError("Error updating display name: " + error.GenerateErrorReport());
    }
    // Send player's score to PlayFab leaderboard
   /* public void SendLeaderboard(int score)
    {
        var request = new UpdatePlayerStatisticsRequest
        {
            Statistics = new List<StatisticUpdate>
            {
                new StatisticUpdate { StatisticName = "HighScore", Value = score }
            }
        };

        PlayFabClientAPI.UpdatePlayerStatistics(request, OnLeaderboardUpdate, OnError);
    }

    void OnLeaderboardUpdate(UpdatePlayerStatisticsResult result)
    {
        Debug.Log("Successfully updated leaderboard!");
    }*/

    // Get the leaderboard from PlayFab
    public void GetLeaderBoard()
    {
        var request = new GetLeaderboardRequest
        {
            StatisticName = "HighScore", // Ensure this matches PlayFab statistic name
            StartPosition = 0,
            MaxResultsCount = 10 // Get top 10 players
        };

        PlayFabClientAPI.GetLeaderboard(request, OnLeaderboardGet, OnError);
    }

    void OnLeaderboardGet(GetLeaderboardResult result)
    {
        // Clear existing leaderboard entries before populating new ones
        foreach (Transform child in rowsParent)
        {
            Destroy(child.gameObject);
        }

        foreach (var item in result.Leaderboard)
        {
            GameObject newGo = Instantiate(rowPrefab, rowsParent);
            TextMeshProUGUI[] texts = newGo.GetComponentsInChildren<TextMeshProUGUI>();
            //earnedvalue = GameManager.Instance.moneyEarned;
            if (texts.Length >= 3) // Ensure we have enough text elements
            {
                texts[0].text = (item.Position + 1).ToString(); // Rank
                texts[1].text = item.DisplayName; // Player ID (Replace with DisplayName later)
                texts[2].text = item.StatValue.ToString(); // Score
            }

            Debug.Log($"Rank: {item.Position + 1} | Player: {item.PlayFabId} | Score: {item.StatValue}");
        }
    }

    // Handle errors
    void OnError(PlayFabError error)
    {
        Debug.LogError("Error: " + error.GenerateErrorReport());
    }

   /* private void OnApplicationQuit()
    {
        SendLeaderboard(GameManager.Instance.moneyEarned);
    }*/
}